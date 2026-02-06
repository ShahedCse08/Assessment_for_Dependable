using PMS.Application.DTOs;
using PMS.Application.Interfaces;
using PMS.Application.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Application.Services
{

    public interface IMedicineService
    {
        List<MedicineDto> GetAllMedicines();
        MedicineDto GetMedicineById(int id);
        OperationResultDto CreateMedicines(MedicineDto dto, int userId);
        OperationResultDto UpdateMedicine(MedicineDto dto, int userId);
        OperationResultDto DeleteMedicine(int id);
        PagedMedicineResponseDto GetMedicinesWithPagination(MedicineQueryRequestDto request);
    }

    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepo;
        public MedicineService(IMedicineRepository medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        public List<MedicineDto> GetAllMedicines()
        {
            return _medicineRepo
                .GetAll()
                .Select(m => m.ToDto())
                .ToList();
        }

        public MedicineDto GetMedicineById(int id)
        {
            var medicine = _medicineRepo.GetById(id);
            return medicine?.ToDto();
        }

        public OperationResultDto CreateMedicines(MedicineDto dto , int userId)
        {
            var validationResult = Validate(dto);
            if (!validationResult.Success)
                return validationResult;
            dto.CreatedBy = userId;
            var medicine = dto.ToDomain();
            return _medicineRepo.CreateMedicines(medicine);
        }

        public OperationResultDto UpdateMedicine(MedicineDto dto, int userId)
        {
            var validationResult = Validate(dto);
            if (!validationResult.Success)
                return validationResult;
            dto.UpdatedBy = userId;
            var medicine = dto.ToDomain();
            return _medicineRepo.UpdateMedicine(medicine);
        }

        public OperationResultDto DeleteMedicine(int id)
        {
            return _medicineRepo.DeleteMedicine(id);
        }

        private OperationResultDto Validate(MedicineDto dto)
        {
            var result = new OperationResultDto();

            if (dto == null)
            {
                result.Success = false;
                result.Message = "Invalid medicine data";
                return result;
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                result.Success = false;
                result.Message = "Medicine name is required";
                return result;
            }

            if (!DateTime.TryParse(dto.ExpiryDate, out var expiry))
            {
                result.Success = false;
                result.Message = "Invalid expiry date";
                return result;
            }

            if (expiry <= DateTime.Now.Date)
            {
                result.Success = false;
                result.Message = "Expiry date must be in the future";
                return result;
            }

            result.Success = true;
            result.Message = string.Empty;
            return result;
        }

        public PagedMedicineResponseDto GetMedicinesWithPagination(MedicineQueryRequestDto request)
        {
            var result = _medicineRepo.GetMedicinesWithPagination(request);
            return result;
        }

    }
}
