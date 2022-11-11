using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class DoctorInfo
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MidName { get; set; }
        public Guid UserId { get; set; }
        public Guid? DoctorDataId { get; set; }
        public string? Speciality { get; set; }
        public Guid? SpecialityId { get; set; }
        public bool? ForDeletion { get; set; }
        public bool? IsBlocked { get; set; }
    }
}
