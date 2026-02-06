using PMS.Domain.Entities;

namespace PMS.Application.DTOs
{
    public class MedicineDto : Audit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BatchNumber { get; set; }
        public string ExpiryDate { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
