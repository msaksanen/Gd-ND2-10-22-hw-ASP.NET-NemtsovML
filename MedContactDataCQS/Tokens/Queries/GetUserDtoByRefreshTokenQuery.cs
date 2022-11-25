using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Tokens.Queries
{
    public class GetUserDtoByRefreshTokenQuery : IRequest<UserDto?>
    {
        public Guid? RefreshToken { get; set; }
    }
}
