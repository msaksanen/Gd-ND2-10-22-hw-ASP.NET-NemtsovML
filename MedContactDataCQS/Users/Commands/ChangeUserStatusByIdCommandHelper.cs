using AutoMapper;
using MedContactDb.Entities;
using MedContactDb;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MedContactDataCQS.Users.Commands
{
    public class ChangeUserStatusByIdCommandHelper : IRequestHandler<ChangeUserStatusByIdCommand, int?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public ChangeUserStatusByIdCommandHelper(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(ChangeUserStatusByIdCommand cmd, CancellationToken cts)
        {
            if (_context.Users != null && _context.Users.Any())
            {
                var entity = await _context.Users.FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.UserId));
                if (entity == null) return -1;
                if (cmd.State == 1) //user is online and logs out
                {
                    entity.IsOnline = false;
                }

                else  //user is offline and logs in
                {
                    entity.IsOnline = true;
                    entity.LastLogin = DateTime.Now;
                }
            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}
