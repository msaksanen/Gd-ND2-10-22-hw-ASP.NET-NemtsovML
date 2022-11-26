using AutoMapper;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Commands
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, int?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public ChangeUserPasswordCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(ChangeUserPasswordCommand cmd, CancellationToken cts)
        {
            if (_context.Users != null && _context.Users.Any())
            {
                var sourceUser = await _context.Users.FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.UserId));
                if (sourceUser != null)
                {
                    sourceUser.PasswordHash = cmd.passwordHash;

                }
            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}
