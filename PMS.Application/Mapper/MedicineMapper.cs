using PMS.Application.DTOs;
using PMS.Domain.Entities;
using System;

namespace PMS.Application.Mapper
{
    public static class MedicineMapper
    {
        public static MedicineDto ToDto(this Medicine medicine)
        {
            if (medicine == null) return null;

            return new MedicineDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                BatchNumber = medicine.BatchNumber,
                ExpiryDate = medicine.ExpiryDate.ToString("yyyy-MM-dd"), 
                Stock = medicine.Stock,
                UnitPrice = medicine.UnitPrice,
                CreatedBy = medicine.CreatedBy,
                UpdatedBy = medicine.UpdatedBy

            };
        }

        public static Medicine ToDomain(this MedicineDto dto)
        {
            if (dto == null) return null;
            return new Medicine
            {
                Id = dto.Id,
                Name = dto.Name,
                BatchNumber = dto.BatchNumber,
                ExpiryDate = DateTime.Parse(dto.ExpiryDate),
                Stock = dto.Stock,
                UnitPrice = dto.UnitPrice,
                CreatedBy = dto.CreatedBy,
                UpdatedBy = dto.UpdatedBy
            };
        }
    }
}
