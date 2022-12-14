using MedContactDb.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.CustomerData.Commands
{
    public class CreateByUserIdCommand : IRequest<CustomerDataDto?>
    {
        public Guid UserId { get; set; }
    }
}
