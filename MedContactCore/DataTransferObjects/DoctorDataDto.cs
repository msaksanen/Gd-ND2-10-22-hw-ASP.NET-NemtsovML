using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class DoctorDataDto 
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }   
        public Guid? UserId { get; set; }
        public bool? IsBlocked { get; set; }
        public Guid? SpecialityId { get; set; }
        public List<DayTimeTable>? DayTimeTables { get; set; }
    }
}
