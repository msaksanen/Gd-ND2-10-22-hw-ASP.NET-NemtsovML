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
        public string? Speciality { get; set; }
        public bool? ForDeletion { get; set; }
    }
}
