using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public class DayTimeTable : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? StartWorkTime { get; set; }
        public DateTime? FinishWorkTime { get; set; }
        public int? ConsultDuration { get; set; }
        public int? TotalTicketQty { get; set; }
        public int? FreeTicketQty { get; set; }
        public Doctor? Doctor { get; set; }
        public Guid? DoctorId { get; set; }
        public List<Appointment>? Appointments { get; set; }

    }
}
