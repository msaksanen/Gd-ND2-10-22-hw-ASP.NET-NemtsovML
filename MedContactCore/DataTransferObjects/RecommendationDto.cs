using MedContactDb.Entities;
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
        public Guid? DoctorDataId { get; set; }
    }
}
