using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartTime { get; set; }
        public Guid? DayTimeTableId { get; set; }
        public Guid? CustomerDataId { get; set; }
      
    }
}
