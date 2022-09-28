using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class Customer: BaseUser
    {
        public List<Appointment>? Appointments { get; set; }
    }
}
