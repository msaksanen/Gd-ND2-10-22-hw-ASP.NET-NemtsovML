using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactDataCQS.Users.Queries
{
    public class GetCDataByIdQuery : IRequest<CustomerDataDto?>
    {
        public Guid? CustomerDataId{ get; set; }
   }
}
