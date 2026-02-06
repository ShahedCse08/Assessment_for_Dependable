namespace PMS.Application.DTOs
{
    public class SalesQueryRequestDto
    {
        public string SearchKeyword { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

}
