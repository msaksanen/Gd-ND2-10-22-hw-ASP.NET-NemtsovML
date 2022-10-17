using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public  class Speciality : IBaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<DoctorData>? DoctorDatas { get; set; }
    }
}
