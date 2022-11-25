using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MedContactDataCQS.Tokens.Commands
{
    public class AddRefreshTokenCommand : IRequest<int?>
    {
        public Guid? TokenValue;
        public Guid? UserId;
    }
}
