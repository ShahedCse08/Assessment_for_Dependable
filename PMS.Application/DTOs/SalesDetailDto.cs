namespace PMS.Application.DTOs
{
    public class SalesDetailDto
    {
        public int Id { get; set; }
        public int SalesMasterId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string BatchNumber { get; set; }
        public string ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
