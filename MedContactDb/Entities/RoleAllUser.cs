using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public  class RoleAllUser : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }
        public Role? Role { get; set; }

        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public Guid? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
      
    }
}
