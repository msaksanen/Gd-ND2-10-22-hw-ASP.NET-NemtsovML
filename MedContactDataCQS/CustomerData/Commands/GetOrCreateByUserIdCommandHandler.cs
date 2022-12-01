using AutoMapper;
using MedContactDataCQS.Family.Commands;
using MedContactDb.Entities;
using MedContactDb;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MedContactDataCQS.CustomerData.Commands
{
    public class GetOrCreateByUserIdCommandHandler : IRequestHandler<GetOrCreateByUserIdCommand, CustomerDataDto?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetOrCreateByUserIdCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerDataDto?> Handle(GetOrCreateByUserIdCommand command, CancellationToken token)
        {
            if (_context.CustomerDatas != null)
            {
                int res = 0;
                var ent = await _context.CustomerDatas.FirstOrDefaultAsync(c => c.UserId.Equals(command.UserId), token);
                if (ent != null)
                {
                    return _mapper.Map<CustomerDataDto>(ent);
                }
                else
                {
                  var newEnt = new MedContactDb.Entities.CustomerData(){ Id = Guid.NewGuid(), IsBlocked = false, UserId = command.UserId};
                  await _context.CustomerDatas.AddAsync(newEnt, token);
                  res = await _context.SaveChangesAsync(token);

                  if (res > 0)
                        return _mapper.Map<CustomerDataDto>(newEnt);
                }
            }
            return null;
        }
    }
}

