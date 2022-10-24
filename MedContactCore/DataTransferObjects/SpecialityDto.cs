using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public  class SpecialityDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool? IsSelected { get; set; }

        //public List<DoctorData>? DoctorDatas { get; set; }
    }
}
