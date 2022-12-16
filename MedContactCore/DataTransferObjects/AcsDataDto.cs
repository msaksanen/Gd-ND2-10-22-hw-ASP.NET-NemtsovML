using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class AcsDataDto
    {
        public Guid Id { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
        public List<User>? Users { get; set; }
        public bool? IsBlocked { get; set; }
        public List<ExtraDataDto>? ExtraDatas { get; set; }

    }
}
