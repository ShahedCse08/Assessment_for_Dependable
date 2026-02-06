using PMS.Domain.Entities;
using System.Collections.Generic;

namespace PMS.Application.DTOs
{
    public class SalesMasterDto : Audit
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Date { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<SalesDetailDto> Details { get; set; }
    }
}
