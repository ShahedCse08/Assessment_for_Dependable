using Dapper;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using System;
using System.Data;
using System.Linq;

namespace PMS.Infrastructure.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly IDbConnection _connection;
        public SalesRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public string GenerateInvoiceNumber()
        {
            var last = _connection.QueryFirstOrDefault<int?>("SELECT MAX(Id) FROM SalesMaster") ?? 0;
            return $"INV-{last + 1:0000}";
        }
        public OperationResultDto CreateInvoiceTransactional(SalesMaster master)
        {
            var result = new OperationResultDto();
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    var masterSql = @"
                INSERT INTO SalesMaster
                    (InvoiceNumber, Date, CustomerName, Contact, SubTotal, Discount, GrandTotal , CreatedBy)
                VALUES
                    (@InvoiceNumber, @Date, @CustomerName, @Contact, @SubTotal, @Discount, @GrandTotal, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    int masterId = _connection.QuerySingle<int>(masterSql, master, tran);

                    foreach (var d in master.Details)
                    {
                        int stock = _connection.QuerySingle<int>(
                            "SELECT Stock FROM Medicines WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id",
                            new { Id = d.MedicineId },
                            tran);

                        if (stock < d.Quantity)
                        {
                            tran.Rollback();
                            result.Message= $"Insufficient stock for {d.MedicineName}. Available: {stock}";
                            result.Success = false;
                            return result;
                        }

                        var detailSql = @"
                    INSERT INTO SalesDetails
                        (SalesMasterId, MedicineId, MedicineName, BatchNumber, ExpiryDate, Quantity, UnitPrice, LineTotal)
                    VALUES
                        (@SalesMasterId, @MedicineId, @MedicineName, @BatchNumber, @ExpiryDate, @Quantity, @UnitPrice, @LineTotal);";

                        d.SalesMasterId = masterId;
                        _connection.Execute(detailSql, d, tran);

                        _connection.Execute(
                            "UPDATE Medicines SET Stock = Stock - @Qty WHERE Id = @Id",
                            new { Qty = d.Quantity, Id = d.MedicineId },
                            tran);
                    }

                    tran.Commit();
                    result.Message = "Invoice created successfully.";
                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = "Error creating invoice: " + ex.Message;
                result.Success = false;
                return result;
            }
        }
        public OperationResultDto UpdateInvoiceTransactional(SalesMaster master)
        {
           var result = new OperationResultDto();

            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    var oldDetails = _connection.Query<SalesDetail>(
                        "SELECT * FROM SalesDetails WHERE SalesMasterId = @Id",
                        new { Id = master.Id },
                        tran).ToList();

                    foreach (var old in oldDetails)
                    {
                        _connection.Execute(
                            "UPDATE Medicines SET Stock = Stock + @Qty WHERE Id = @Id",
                            new { Qty = old.Quantity, Id = old.MedicineId },
                            tran);
                    }

                    foreach (var d in master.Details)
                    {
                        int stock = _connection.QuerySingle<int>(
                            "SELECT Stock FROM Medicines WITH (UPDLOCK, ROWLOCK) WHERE Id = @Id",
                            new { Id = d.MedicineId },
                            tran);

                        if (stock < d.Quantity)
                        {
                            tran.Rollback();
                            result.Message = $"Insufficient stock for {d.MedicineName}. Available: {stock}";
                            result.Success = false;
                            return result;
                        }
                    }

                    var masterSql = @"
                    UPDATE SalesMaster SET
                    Date = @Date,
                    CustomerName = @CustomerName,
                    Contact = @Contact,
                    Discount = @Discount,
                    SubTotal = @SubTotal,
                    GrandTotal = @GrandTotal,
                    Updatedby = @Updatedby,
                    UpdatedAt = GETDATE()
                    WHERE Id = @Id";

                    _connection.Execute(masterSql, master, tran);

                    _connection.Execute(
                        "DELETE FROM SalesDetails WHERE SalesMasterId = @Id",
                        new { Id = master.Id },
                        tran);

                    foreach (var d in master.Details)
                    {
                        d.SalesMasterId = master.Id;

                        var detailSql = @"
                    INSERT INTO SalesDetails
                        (SalesMasterId, MedicineId, MedicineName, BatchNumber, ExpiryDate, Quantity, UnitPrice, LineTotal)
                    VALUES
                        (@SalesMasterId, @MedicineId, @MedicineName, @BatchNumber, @ExpiryDate, @Quantity, @UnitPrice, @LineTotal);";

                        _connection.Execute(detailSql, d, tran);

                        _connection.Execute(
                            "UPDATE Medicines SET Stock = Stock - @Qty WHERE Id = @Id",
                            new { Qty = d.Quantity, Id = d.MedicineId },
                            tran);
                    }

                    tran.Commit();
                    result.Message = "Invoice updated successfully.";
                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = "Error updating invoice: " + ex.Message;
                result.Success = false;
                return result;
            }
        }
        public OperationResultDto DeleteInvoiceTransactional(int invoiceId)
        {
            var result = new OperationResultDto();

            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using (var tran = _connection.BeginTransaction())
                {
                    var details = _connection.Query<SalesDetail>("SELECT * FROM SalesDetails WHERE SalesMasterId = @Id", new { Id = invoiceId }, tran).ToList();

                    if (details.Count == 0)
                    {
                        tran.Rollback();
                        result.Message = "Invoice not found or already deleted.";
                        result.Success = false;
                        return result;
                    }

                    foreach (var item in details)
                    {
                        _connection.Execute("UPDATE Medicines SET Stock = Stock + @Qty WHERE Id = @Id", new { Qty = item.Quantity, Id = item.MedicineId }, tran);
                    }

                    _connection.Execute("DELETE FROM SalesDetails WHERE SalesMasterId = @Id", new { Id = invoiceId }, tran);
                    _connection.Execute("DELETE FROM SalesMaster WHERE Id = @Id", new { Id = invoiceId }, tran);

                    tran.Commit();
                    result.Message = "Invoice deleted successfully.";
                    result.Success = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = "Error deleting invoice: " + ex.Message;
                result.Success = false;
                return result;
            }
        }
        public PagedSalesResponseDto GetSalesWithPagination(SalesQueryRequestDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", request.PageIndex + 1);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@SearchKeyword", string.IsNullOrWhiteSpace(request.SearchKeyword) ? null : request.SearchKeyword);

            var salesMasters = _connection.Query<SalesMasterResponseDto>("GetSalesWithPagination", parameters, commandType: CommandType.StoredProcedure).ToList();

            int totalCount = salesMasters.FirstOrDefault()?.TotalCount ?? 0;
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PagedSalesResponseDto
            {
                Sales = salesMasters,
                PageNumber = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
        public SalesMaster GetById(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@InvoiceId", id);

            using (var multi = _connection.QueryMultiple("GetInvoiceById", parameters,
                   commandType: CommandType.StoredProcedure))
            {
                var master = multi.ReadFirstOrDefault<SalesMaster>();
                var details = multi.Read<SalesDetail>();
                master.Details = details.ToList();
                return master;
            }
        }
    }
}
