using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public class CheckUserPwdHashByUserIdQuery : IRequest<bool?>
    {
        public Guid? UserId { get; set; }
        public string? passwordHash { get; set; }
    }
}
