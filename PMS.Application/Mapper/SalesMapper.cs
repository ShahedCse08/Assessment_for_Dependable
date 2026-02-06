using PMS.Application.DTOs;
using PMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Application.Mapper
{
    public static class SalesMapper
    {
        public static SalesMasterDto ToDto(this SalesMaster master)
        {
            if (master == null) return null;

            return new SalesMasterDto
            {
                Id = master.Id,
                InvoiceNumber = master.InvoiceNumber,
                Date = master.Date.ToString("yyyy-MM-dd"),
                CustomerName = master.CustomerName,
                Contact = master.Contact,
                SubTotal = master.SubTotal,
                Discount = master.Discount,
                GrandTotal = master.GrandTotal,
                CreatedBy = master.CreatedBy,
                UpdatedBy = master.UpdatedBy,
                Details = master.Details?.Select(d => d.ToDto()).ToList() ?? new List<SalesDetailDto>()
            };
        }

        public static SalesMaster ToDomain(this SalesMasterDto dto)
        {
            if (dto == null) return null;

            return new SalesMaster
            {
                Id = dto.Id,
                InvoiceNumber = dto.InvoiceNumber,
                Date = DateTime.Parse(dto.Date),
                CustomerName = dto.CustomerName,
                Contact = dto.Contact,
                SubTotal = dto.SubTotal,
                Discount = dto.Discount,
                GrandTotal = dto.GrandTotal,
                CreatedBy = dto.CreatedBy,
                UpdatedBy = dto.UpdatedBy,
                Details = dto.Details?.Select(d => d.ToDomain()).ToList() ?? new List<SalesDetail>()
            };
        }

        public static SalesDetailDto ToDto(this SalesDetail detail)
        {
            if (detail == null) return null;

            return new SalesDetailDto
            {
                Id = detail.Id,
                SalesMasterId = detail.SalesMasterId,
                MedicineId = detail.MedicineId,
                MedicineName = detail.MedicineName,
                BatchNumber = detail.BatchNumber,
                ExpiryDate = detail.ExpiryDate.ToString("dd-MMM-yyyy"),
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                LineTotal = detail.LineTotal
            };
        }

        public static SalesDetail ToDomain(this SalesDetailDto dto)
        {
            if (dto == null) return null;

            return new SalesDetail
            {
                Id = dto.Id,
                SalesMasterId = dto.SalesMasterId,
                MedicineId = dto.MedicineId,
                MedicineName = dto.MedicineName,
                BatchNumber = dto.BatchNumber,
                ExpiryDate = DateTime.Parse(dto.ExpiryDate),
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                LineTotal = dto.LineTotal
            };
        }
    }
}
