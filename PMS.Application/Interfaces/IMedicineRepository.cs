using PMS.Application.DTOs;
using PMS.Domain.Entities;
using System.Collections.Generic;

namespace PMS.Application.Interfaces
{
    public interface IMedicineRepository
    {
        OperationResultDto CreateMedicines(Medicine medicine);
        OperationResultDto UpdateMedicine(Medicine medicine);
        OperationResultDto DeleteMedicine(int medicineId);
        IEnumerable<Medicine> GetAll();
        Medicine GetById(int id);
        PagedMedicineResponseDto GetMedicinesWithPagination(MedicineQueryRequestDto request);


    }
}
