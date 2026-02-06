using System;
using System.Collections.Generic;

namespace PMS.Application.DTOs
{
    public class SalesMasterResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<SalesDetailResponseDto> Details { get; set; } = new List<SalesDetailResponseDto>();
        public int TotalCount { get; set; }
    }
}
