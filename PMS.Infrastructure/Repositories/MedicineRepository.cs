using Dapper;
using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace PMS.Infrastructure.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly IDbConnection _connection;
        public MedicineRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public OperationResultDto CreateMedicines(Medicine medicine)
        {
            var result = new OperationResultDto();

            try
            {
                var checkSql = @"SELECT COUNT(1) FROM Medicines 
                       WHERE Name = @Name AND BatchNumber = @BatchNumber";

                var exists = _connection.QuerySingle<int>(checkSql, new
                {
                    medicine.Name,
                    medicine.BatchNumber
                }) > 0;

                if (exists)
                {
                    result.Message = $"Medicine with Name '{medicine.Name}' And BatchNumber '{medicine.BatchNumber}' already exists.";
                    result.Success = false;
                    return result;
                }

                var insertSql = @"INSERT INTO Medicines (Name, BatchNumber, ExpiryDate, Stock, UnitPrice , CreatedBy)
                        VALUES (@Name, @BatchNumber, @ExpiryDate, @Stock, @UnitPrice ,@CreatedBy);
                        SELECT CAST(SCOPE_IDENTITY() as int)";

                var newId = _connection.QuerySingle<int>(insertSql, medicine);

                result.Message = $"Medicine created successfully with ID: {newId}";
                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = $"Error creating medicine: {ex.Message}";
                result.Success = false;
                return result;
            }
        }
        public OperationResultDto UpdateMedicine(Medicine medicine)
        {
            var result = new OperationResultDto();

            try
            {
                var updateSql = @"
                UPDATE Medicines 
                SET Name = @Name, 
                BatchNumber = @BatchNumber, 
                ExpiryDate = @ExpiryDate, 
                Stock = @Stock, 
                UnitPrice = @UnitPrice ,
                Updatedby = @UpdatedBy,
                UpdatedAt = GETDATE()
                WHERE Id = @Id";

                var rowsAffected = _connection.Execute(updateSql, medicine);

                if (rowsAffected > 0)
                {
                    result.Message = $"Medicine with ID '{medicine.Id}' updated successfully.";
                    result.Success = true;
                    return result;
                }
                else
                {
                    result.Message = $"Failed to update medicine with ID '{medicine.Id}'.";
                    result.Success = false;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Error updating medicine: {ex.Message}";
                result.Success = false;
                return result;
            }
        }
        public OperationResultDto DeleteMedicine(int medicineId)
        {
            var result = new OperationResultDto();

            try
            {
                var checkSql = "SELECT COUNT(1) FROM Medicines WHERE Id = @Id";
                var exists = _connection.QuerySingle<int>(checkSql, new { Id = medicineId }) > 0;
                if (!exists)
                {
                    result.Message = $"Medicine with ID '{medicineId}' does not exist.";
                    result.Success = false;
                    return result;
                }
                var salesCheckSql = "SELECT COUNT(1) FROM SalesDetails WHERE MedicineId = @MedicineId";
                var usedInSales = _connection.QuerySingle<int>(salesCheckSql, new { MedicineId = medicineId }) > 0;

                if (usedInSales)
                {
                    result.Message = "This medicine cannot be deleted because it is already used in sales records.";
                    result.Success = false;
                    return result;
                }

                var deleteSql = "DELETE FROM Medicines WHERE Id = @Id";
                var rowsAffected = _connection.Execute(deleteSql, new { Id = medicineId });

                if (rowsAffected > 0)
                {
                    result.Message = $"Medicine with ID '{medicineId}' deleted successfully.";
                    result.Success = true;
                    return result;
                }

                result.Message = $"Failed to delete medicine with ID '{medicineId}'.";
                result.Success = false;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = $"Error deleting medicine: {ex.Message}";
                result.Success = false;
                return result;
            }
        }
        public IEnumerable<Medicine> GetAll()
        {
            return _connection.Query<Medicine>("SELECT * FROM Medicines");
        }
        public PagedMedicineResponseDto GetMedicinesWithPagination(MedicineQueryRequestDto request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", request.PageIndex + 1);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@SearchKeyword",
                string.IsNullOrWhiteSpace(request.SearchKeyword)
                    ? null
                    : request.SearchKeyword);

            var medicines = _connection
                .Query<MedicineResponseDto>(
                    "GetMedicinesWithPagination",
                    parameters,
                    commandType: CommandType.StoredProcedure)
                .ToList();

            int totalCount = medicines.FirstOrDefault()?.TotalCount ?? 0;
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new PagedMedicineResponseDto
            {
                Medicines = medicines,
                PageNumber = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
        public Medicine GetById(int id)
        {
            return _connection.QueryFirstOrDefault<Medicine>(
                "SELECT * FROM Medicines WHERE Id = @Id", new { Id = id });

        }
       




    }
}