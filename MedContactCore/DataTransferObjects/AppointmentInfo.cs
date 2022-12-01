using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class AppointmentInfo
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid? DayTimeTableId { get; set; }
        public Guid? DoctorDataId { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorMidname { get; set; }
        public string? DoctorSurname { get; set; }
        public string? DoctorSpeciality { get; set; }
        public Guid? CustomerDataId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerMidname { get; set; }
        public string? CustomerSurname { get; set; }
        public DateTime? CustomerBirthDate { get; set; }


    }
}
