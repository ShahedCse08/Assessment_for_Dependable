using PMS.Application.DTOs;
using PMS.Domain.Entities;

namespace PMS.Application.Interfaces
{
    public interface ISalesRepository
    {
        string GenerateInvoiceNumber();
        OperationResultDto CreateInvoiceTransactional(SalesMaster master);
        OperationResultDto UpdateInvoiceTransactional(SalesMaster master);
        OperationResultDto DeleteInvoiceTransactional(int invoiceId);
        PagedSalesResponseDto GetSalesWithPagination(SalesQueryRequestDto request);
        SalesMaster GetById(int id);
    }
}
