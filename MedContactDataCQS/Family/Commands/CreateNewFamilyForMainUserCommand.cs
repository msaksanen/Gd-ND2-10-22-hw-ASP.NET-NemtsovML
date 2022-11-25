using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Family.Commands
{
    public  class CreateNewFamilyForMainUserCommand : IRequest<int?>
    {
        public UserDto? MainUserDto { get; set; } 
        public FamilyDto? FamilyDto { get; set; }
    }
}
