namespace MedContactApp.FilterSortHelpers
{
    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public string PageRoute { get; } = string.Empty;
       
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PageViewModel(int count, int pageNumber, int pageSize, string pageRoute)
        {
            PageNumber = pageNumber;
            PageRoute = pageRoute;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
