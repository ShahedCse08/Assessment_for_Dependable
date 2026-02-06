using System;
using System.Collections.Generic;

namespace PMS.Domain.Entities
{
    public class SalesMaster : Audit
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<SalesDetail> Details { get; set; } = new List<SalesDetail>();
    }
}
