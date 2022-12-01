using AutoMapper;
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
    public class RemoveApmByIdCommandHandler : IRequestHandler<RemoveApmByIdCommand, int>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public RemoveApmByIdCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(RemoveApmByIdCommand command, CancellationToken token)
        {
            int res = 0;

            if (_context.DayTimeTables != null && _context.Appointments != null)
            {
                var ent = await _context.Appointments.FirstOrDefaultAsync(d => d.Id.Equals(command.ApmId), token);

                if (ent != null && ent.DayTimeTableId!=null)
                {
                   var dtt = await _context.DayTimeTables.FirstOrDefaultAsync(d => d.Id.Equals(ent.DayTimeTableId), token);
                   if (dtt != null) dtt.FreeTicketQty++;
                   _context.Appointments.Remove(ent);
                   res = await _context.SaveChangesAsync(token);
                }

            }
            return res;
        }
    }
}
