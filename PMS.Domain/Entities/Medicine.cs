using System;


namespace PMS.Domain.Entities
{
    public class Medicine : Audit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
