using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DayTimeTable.Queries;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Appointment.Queries
{
    public class GetAppointmentsByDTTableIdQueryHandler :IRequestHandler<GetAppointmentsByDTTableIdQuery, List<AppointmentDto>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetAppointmentsByDTTableIdQueryHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

        public async Task<List<AppointmentDto>?> Handle(GetAppointmentsByDTTableIdQuery request,
        CancellationToken cts)
        {
        if (_context.Appointments != null)
        {
                var list = await _context.Appointments
                                           .Include(a => a.CustomerData)
                                           .ThenInclude(c => c!.User)
                                           .Where(a => a.DayTimeTableId != null && a.DayTimeTableId.Equals(request.DayttId))
                                           .Select(a => _mapper.Map<AppointmentDto>(a))
                                           .ToListAsync();
                return list;
            }
        return null;
        }
    }
}
