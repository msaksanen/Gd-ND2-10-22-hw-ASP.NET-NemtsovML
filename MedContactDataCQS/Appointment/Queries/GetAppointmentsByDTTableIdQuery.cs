using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Appointment.Queries
{
    public class GetAppointmentsByDTTableIdQuery : IRequest<List<AppointmentDto>?>
    {
        public Guid DayttId { get; set; }
    }
}
