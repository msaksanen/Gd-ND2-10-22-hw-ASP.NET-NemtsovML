using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class RoleDto 
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<User>? Users { get; set; }
        public List<DoctorDataDto>? DoctorDatas { get; set; }
        public List<CustomerDataDto>? CustomerDatas { get; set; }
        public List<AcsDataDto>? AcsDatas { get; set; }
       

    }
}
