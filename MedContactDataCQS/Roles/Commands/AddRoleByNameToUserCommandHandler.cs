using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Roles.Queries;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Roles.Commands
{
    public class AddRoleByNameToUserCommandHandler : IRequestHandler<AddRoleByNameToUserCommand, int>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public AddRoleByNameToUserCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> Handle(AddRoleByNameToUserCommand cmd, CancellationToken cts)
        {
            int result = 0;
            if (_context.Roles != null && _context.Roles.Any() &&
                _context.Users != null && _context.Users.Any())
            {
                var list = await _context.Roles
                          .AsNoTracking()
                          .Include(r => r.Users)
                          .AsNoTracking()
                          .ToListAsync(cts);

                var userRoleList = from role in list
                          from user in role.Users!
                          where user.Id.Equals(cmd.UserId)
                          select _mapper.Map<RoleDto>(role);

                if (userRoleList != null && !userRoleList.Any(r => r.Name != null && r.Name.Equals(cmd.RoleName)))
                {
                    var role = await _context.Roles
                                      .FirstOrDefaultAsync(r => r.Name != null && r.Name.Equals(cmd.RoleName),cts);

                    var user = await _context.Users.FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.UserId),cts);

                    _context.Roles.Include(r => r.Users).Load();

                    if (role != null && user != null)
                    {
                        role.Users?.Add(user);
                        result = await _context.SaveChangesAsync(cts);
                    }
                }
            }
            return result;
        }
    }

}
