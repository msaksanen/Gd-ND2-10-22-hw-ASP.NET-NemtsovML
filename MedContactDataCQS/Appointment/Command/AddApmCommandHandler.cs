using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DayTimeTable.Commands;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Appointment.Command
{
    public class AddApmCommandHandler : IRequestHandler<AddApmCommand, int>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public AddApmCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddApmCommand command, CancellationToken token)
        {
            int res = 0;
            
            if (_context.DayTimeTables != null && _context.Appointments != null)
            {
                var ent = _mapper.Map<MedContactDb.Entities.Appointment>(command.Apm);

                await _context.Appointments.AddAsync(ent, token);
                if (ent!=null && ent.DayTimeTableId != null)
                {
                    var dtt = await _context.DayTimeTables.FirstOrDefaultAsync(d => d.Id.Equals(ent.DayTimeTableId), token);
                    if (dtt != null) dtt.FreeTicketQty--;
                    res = await _context.SaveChangesAsync(token);
                }
            }
            return res;
        }
    }
}
