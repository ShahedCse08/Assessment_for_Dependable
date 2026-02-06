using System.Collections.Generic;

namespace PMS.Application.DTOs
{
    public class PagedMedicineResponseDto
    {
        public List<MedicineResponseDto> Medicines { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
