using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class RefreshToken : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
