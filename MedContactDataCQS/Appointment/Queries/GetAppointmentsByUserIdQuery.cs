using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Appointment.Queries
{
    public class GetAppointmentsByUserIdQuery : IRequest <IEnumerable<AppointmentDto>?>
    {
        public Guid UsrId { get; set; }
    }
}
