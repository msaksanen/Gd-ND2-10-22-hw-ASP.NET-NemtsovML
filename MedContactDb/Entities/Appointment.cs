using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactDb.Entities
{
    public class Appointment : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartTime { get; set; }
        public Guid? CustomerDataId { get; set; }
        public CustomerData? CustomerData { get; set; }
        public Guid? DayTimeTableId { get; set; }
        public DayTimeTable? DayTimeTable { get; set; }
    }
}
