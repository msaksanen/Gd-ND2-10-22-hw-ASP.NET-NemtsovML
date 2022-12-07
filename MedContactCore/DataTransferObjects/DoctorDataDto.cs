using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects

{
    public class DoctorDataDto 
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }   
        public Guid? UserId { get; set; }
        public UserDto? User { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? ForDeletion { get; set; }
        public string? SpecNameReserved { get; set; }
        public Guid? SpecialityId { get; set; }
        public SpecialityDto? Speciality { get; set; }
        public List<DayTimeTableDto>? DayTimeTables { get; set; }
        public List<MedDataDto>? MedData { get; set; }
    }
}
