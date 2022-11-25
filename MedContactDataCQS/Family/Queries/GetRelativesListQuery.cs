using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Family.Queries
{
    public class GetRelativesListQuery : IRequest<List<UserDto>?>
    {
        public Guid? MainUserId { get; set; }
    }
}
