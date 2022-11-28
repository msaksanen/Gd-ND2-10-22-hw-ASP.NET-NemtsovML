using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Roles.Commands
{
    public class AddRoleByNameToUserCommand : IRequest<int>
    {
        public Guid? UserId { get; set; } 
        public string? RoleName { get; set; }
    }
}
