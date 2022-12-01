using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// AppointmentIndexCreateModel
    /// </summary>
    public class AppointmentIndexCreateModel
    {
        ///
        public DayTimeTableDto? DayTimeTable { get; set; }
        ///
        public IEnumerable<AppointmentDto>? Appointments { get; set; }
        ///
        public DoctorInfo? DoctorInfo { get; set; }
        ///
        public UserDto? User { get; set; }  
    }
}
