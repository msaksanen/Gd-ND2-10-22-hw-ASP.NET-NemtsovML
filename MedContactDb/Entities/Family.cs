using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class Family : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid? MainUserId { get; set; }
        List<User>? Users { get; set; }
    }
}
