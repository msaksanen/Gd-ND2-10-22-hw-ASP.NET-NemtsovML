using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public class GetUserDtoByEmailPwdHashQueryHandler : IRequestHandler<GetUserDtoByEmailPwdHashQuery, UserDto?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetUserDtoByEmailPwdHashQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserDtoByEmailPwdHashQuery request,
            CancellationToken cts)

        {

            if (_context.Users != null && _context.Users.Any())
            {
              var user  = await _context.Users
                        .FirstOrDefaultAsync(u => u.IsDependent != true
                        && u.Email != null && u.Email.Equals(request.Email)
                        && u.PasswordHash != null && u.PasswordHash.Equals(request.PasswordHash));
             return _mapper.Map<UserDto>(user);
            }

            return null;
        }
    }
}
