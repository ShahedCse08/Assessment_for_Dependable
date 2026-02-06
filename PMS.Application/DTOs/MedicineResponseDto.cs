using System;

namespace PMS.Application.DTOs
{
    public class MedicineResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        public int TotalCount { get; set; } 
    }
}
