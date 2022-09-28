using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class RecommendationDto : MedDataDto 
    { 
        public Guid? DoctorId { get; set; }
        public DoctorDto? Doctor { get; set; }
    }
}
