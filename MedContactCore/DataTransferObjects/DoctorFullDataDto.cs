using MedContactCore.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects

{
    public class DoctorFullDataDto 
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }   
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Surname { get; set; }
        public string? MidName { get; set; }
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? IsFullBlocked { get; set; }
        public bool? ForDeletion { get; set; }
        public Guid? SpecialityId { get; set; }
        public string? SpecialityName { get; set; }
        public string? SpecNameReserved { get; set; }
        //public List<DayTimeTableDto>? DayTimeTables { get; set; }
        // public List<RoleDto>? Roles { get; set; }
        public List<SelectItem>? Roles { get; set; }
    }
}
