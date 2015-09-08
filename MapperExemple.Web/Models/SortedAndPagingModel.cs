namespace MapperExemple.Web.Models
{
    public class SortedAndPagingModel
    {
        public string SortField { get; set; }
        public string SortDirection { get; set; }

        public int PageIndex { get; set; }

        public double NumberOfPage { get; set; }
    }
}