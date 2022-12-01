using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Appointment.Command
{
    public class RemoveApmByIdCommand : IRequest<int>
    {
        public Guid? ApmId { get; set; }    
    }
}
