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
    public class RemoveUserByIdCommandHandler : IRequestHandler<RemoveUserByIdCommand, int>
    {
        private readonly MedContactContext _context;
    private readonly IMapper _mapper;

    public RemoveUserByIdCommandHandler (MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(RemoveUserByIdCommand cmd, CancellationToken cts)
    {
       int res = 0;
        if (_context.Users != null && _context.Users.Any())
        {
                var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(cmd.Id), cts);
                if (entity != null)
                {
                    _context.Users.Remove(entity); 
                    res = await _context.SaveChangesAsync(cts);
                }
            }
      
        return res;
    }
}
}
