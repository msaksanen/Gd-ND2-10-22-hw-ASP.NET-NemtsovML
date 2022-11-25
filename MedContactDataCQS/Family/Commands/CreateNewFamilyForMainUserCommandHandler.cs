using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Tokens.Commands;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Family.Commands
{
    public class CreateNewFamilyForMainUserCommandHandler
      : IRequestHandler<CreateNewFamilyForMainUserCommand, int?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public CreateNewFamilyForMainUserCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(CreateNewFamilyForMainUserCommand command, CancellationToken token)
        {
            int res = 0;
            if (_context.Users != null && _context.Users.Any() &&
                _context.Families != null && _context.Families.Any())
            {
                var mainUser = _mapper.Map<User>(command.MainUserDto);
                var family = _mapper.Map<MedContactDb.Entities.Family>(command.FamilyDto);

                _context.Users.Update(mainUser);
                await _context.Families.AddAsync(family, token);
                res  = await _context.SaveChangesAsync(token);
            }
            return res;     
        }
    }
}
