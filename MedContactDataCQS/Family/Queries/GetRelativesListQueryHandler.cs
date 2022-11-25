using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Family.Queries;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Roles.Queries
{
    public class GetRelativesListQueryHandler : IRequestHandler<GetRelativesListQuery, List<UserDto>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetRelativesListQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDto>?> Handle(GetRelativesListQuery request,
            CancellationToken cts)
        {
            if (_context.Families != null && _context.Families.Any() &&
                _context.Users!=null && _context.Users.Any())
            {
                var family = await _context.Families
                            .FirstOrDefaultAsync(f => f.MainUserId.Equals(request.MainUserId));

                if (family != null)
                {
                    var list = await _context.Users
                                   .Where(u => u.FamilyId.Equals(family!.Id))
                                   .Select(customer => _mapper.Map<UserDto>(customer))
                                   .ToListAsync();

                    return list;
                }

                return null;
            }
            return null;
        }
    }
}
