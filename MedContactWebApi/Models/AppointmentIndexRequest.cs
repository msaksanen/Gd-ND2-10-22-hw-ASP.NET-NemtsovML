using MedContactWebApi.FilterSortPageHelpers;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// AppointmentIndexRequest
    /// </summary>
    public class AppointmentIndexRequest
    {
        /// DayTimeTableId
        public string Id { get; set; } =String.Empty;
        ///
        public string Name { get; set; } =String.Empty;
        ///
        public string Birthdate { get; set; } =String.Empty;
        ///
        public string SysInfo { get; set; } =String.Empty;
        ///
        public SortState SortOrder { get; set; } = SortState.DateAsc;
    }
}
