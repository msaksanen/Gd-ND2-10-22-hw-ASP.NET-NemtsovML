using MedContactWebApi.FilterSortPageHelpers;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// TimeTableDoctIndexRqstModel for TimeTableDoctIndex
    /// </summary>
    public class TimeTableDoctIndexRqstModel
    {
       ///
       public string Dataid { get; set; } = string.Empty;
       /// 
       public string Uid { get; set; } = string.Empty;
       /// 
       public string RefLink { get; set; } = string.Empty;
       ///
       public int Page { get; set; } = 1;
       ///
       public int? PageSize { get; set; }
       ///
       public SortState SortOrder { get; set; } = SortState.DateDesc;
    }
}
