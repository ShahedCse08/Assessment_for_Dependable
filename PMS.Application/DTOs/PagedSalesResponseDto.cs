using System.Collections.Generic;

namespace PMS.Application.DTOs
{
    public class PagedSalesResponseDto
    {
        public List<SalesMasterResponseDto> Sales { get; set; } = new List<SalesMasterResponseDto>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
     
    }
}
