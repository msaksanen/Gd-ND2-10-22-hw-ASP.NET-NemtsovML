using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactDb.Entities
{
    public class DoctorData : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? ForDeletion { get; set; }
        public Guid? SpecialityId { get; set; }
        public Speciality? Speciality { get; set; }
        public List<DayTimeTable>? DayTimeTables { get; set; }
    }
}
