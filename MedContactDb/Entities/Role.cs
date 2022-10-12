using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class Role  : IBaseEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public List<User>? Users { get; set; }
        public List<Doctor>? Doctors { get; set; }
        public List<Customer>? Customers { get; set; }
        public List<RoleAllUser>? RoleAllUsers { get; set; }

    }
}
