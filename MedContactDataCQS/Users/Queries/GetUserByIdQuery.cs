using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedContactCore.DataTransferObjects;

namespace MedContactDataCQS.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto?>
    {
        public Guid? UserId { get; set; }
   }
}
