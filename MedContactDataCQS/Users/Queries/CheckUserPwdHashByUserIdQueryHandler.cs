using AutoMapper;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public class CheckUserPwdHashByUserIdQueryHandler : IRequestHandler<CheckUserPwdHashByUserIdQuery, bool?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public CheckUserPwdHashByUserIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool?> Handle(CheckUserPwdHashByUserIdQuery request,
            CancellationToken cts)

        {
            if (_context.Users != null && _context.Users.Any() )
            {
                var dbPasswordHash = (await _context.Users
                              .FirstOrDefaultAsync(user => user.Id.Equals(request.UserId)))?.PasswordHash;

                return
                   dbPasswordHash != null  && request?.passwordHash?.Equals(dbPasswordHash)==true;

            }
            return null;
        }
    }
}
