using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.Mapper;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Application.Services
{
    public interface ISalesService
    {
        string GenerateInvoiceNumber();
        List<MedicineDto> GetAllMedicines();
        OperationResultDto CreateInvoice(SalesMasterDto dto, int userId);
        OperationResultDto UpdateInvoice(SalesMasterDto dto, int userId);
        OperationResultDto DeleteInvoice(int invoiceId);
        SalesMasterDto GetById(int id);
        PagedSalesResponseDto GetSalesWithPagination(SalesQueryRequestDto request);
    }
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepo;
        private readonly IMedicineRepository _medicineRepo;
        public SalesService(ISalesRepository salesRepo, IMedicineRepository medicineRepo)
        {
            _salesRepo = salesRepo;
            _medicineRepo = medicineRepo;
        }
        public string GenerateInvoiceNumber()
        {
            return _salesRepo.GenerateInvoiceNumber();
        }
        public List<MedicineDto> GetAllMedicines()
        {
            return _medicineRepo.GetAll().Select(m => m.ToDto()).ToList();
        }
        public OperationResultDto CreateInvoice(SalesMasterDto dto, int userId)
        {
            if (dto == null)
            {
                return new OperationResultDto { Success = false, Message = "Invalid invoice data." };
            }
            dto.CreatedBy = userId;
            var master = dto.ToDomain();
            return _salesRepo.CreateInvoiceTransactional(master);
        }
        public OperationResultDto UpdateInvoice(SalesMasterDto dto, int userId)
        {
            if (dto == null || dto.Id <= 0)
            {
                return new OperationResultDto { Success = false, Message = "Invalid invoice data." };
            }
            dto.UpdatedBy = userId;
            var master = dto.ToDomain();
            return _salesRepo.UpdateInvoiceTransactional(master);
        }
        public OperationResultDto DeleteInvoice(int invoiceId)
        {
            return _salesRepo.DeleteInvoiceTransactional(invoiceId);
        }
        public SalesMasterDto GetById(int id)
        {
            if (id <= 0) return null;

            var master = _salesRepo.GetById(id);
            return master?.ToDto();
        }
        public PagedSalesResponseDto GetSalesWithPagination(SalesQueryRequestDto request)
        {
            if (request == null) return new PagedSalesResponseDto();
            return _salesRepo.GetSalesWithPagination(request);
        }

    }
}
