using MedContactWebApi.FilterSortPageHelpers;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// MyApmsRequestModel
    /// </summary>
    public class MyApmsRequestModel
    {
        /// 
        public string Name { get; set; } = String.Empty;
        /// 
        public string Speciality { get; set; } = String.Empty;
        ///
        public string Date { get; set; } = String.Empty;
        /// 
        public string SysInfo { get; set; } = String.Empty;
        /// 
        public SortState SortOrder { get; set; } = SortState.DateDesc;
    }
}
