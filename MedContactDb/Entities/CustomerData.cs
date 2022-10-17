using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class CustomerData : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public bool? IsBlocked { get; set; }
        public List<Appointment>? Appointments { get; set; }
        public List<MedData>? MedData { get; set; }
    }
}
