namespace MedContactWebApi.Models
{
    /// <summary>
    /// AppointmentRequestModel
    /// </summary>
    public class AppointmentRequestModel
    {
        /// DayTimeTableId
        public string? Dttid { get; set; }
        /// CustomerDataId
        public string? Cdid { get; set; }
        ///StartTime
        public string? Stime { get; set; }
    }
}
