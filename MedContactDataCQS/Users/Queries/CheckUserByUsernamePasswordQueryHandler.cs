using AutoMapper;
using MedContactCore.DataTransferObjects;
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
    public class CheckUserByUsernamePasswordQueryHandler :IRequestHandler<CheckUserByUsernamePasswordQuery, int>
    {
        private readonly MedContactContext _context;
    private readonly IMapper _mapper;

    public CheckUserByUsernamePasswordQueryHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<int> Handle(CheckUserByUsernamePasswordQuery request,
    CancellationToken cts)
    {
        if (_context.Users != null && _context.Users.Any() )
        {
                var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(request.Id), cts);

                if (entity == null)
                    return 0;
                if (entity.PasswordHash == request.PwdHash && entity.Username == request.Username)
                    return 2;

                return 1;
            }
        return 0;
    }
}
}
