using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class Recommendation : MedData 
    { 
        public Guid? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
