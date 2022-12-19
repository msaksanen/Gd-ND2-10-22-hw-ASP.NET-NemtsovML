namespace MedContactWebApi.FilterSortPageHelpers
{
    /// <summary>
    /// PageViewModel
    /// </summary>
    public class PageViewModel
    {
        ///
        public int PageNumber { get; }
        ///
        public int TotalPages { get; }
        ///
        public string PageRoute { get; } = string.Empty;

        ///
        public bool HasPreviousPage => PageNumber > 1;
        ///
        public bool HasNextPage => PageNumber < TotalPages;
        /// 
        public int Count { get; }
        ///
        public int PageSize { get; }
        /// <summary>
        /// PageViewModel Ctor
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageRoute"></param>
        public PageViewModel(int count, int pageNumber, int pageSize, string pageRoute)
        {
            PageNumber = pageNumber;
            PageRoute = pageRoute;
            Count = count;
            PageSize = pageSize;    
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
