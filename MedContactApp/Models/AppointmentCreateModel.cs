using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class AppointmentCreateModel
    {
        public Guid DayTimeTableId { get; set; }       
        public int? Result { get; set; }
        public AppointmentDto? Appointment { get; set; }
        public DoctorInfo? DoctorInfo { get; set; }
        public UserDto? User { get; set; }
    }
}
