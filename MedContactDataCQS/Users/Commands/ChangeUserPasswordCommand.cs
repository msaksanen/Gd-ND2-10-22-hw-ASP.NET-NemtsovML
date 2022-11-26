using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Commands
{
    public class ChangeUserPasswordCommand : IRequest<int?>
    {
        public Guid? UserId { get; set; }
        public string? passwordHash { get; set; }
    }
}
