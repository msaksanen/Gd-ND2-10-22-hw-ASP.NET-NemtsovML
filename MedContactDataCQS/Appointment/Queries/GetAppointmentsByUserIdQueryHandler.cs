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

namespace MedContactDataCQS.Appointment.Queries
{
    public class GetAppointmentsByUserIdQueryHandler : IRequestHandler<GetAppointmentsByUserIdQuery, IEnumerable<AppointmentDto>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetAppointmentsByUserIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentDto>?> Handle(GetAppointmentsByUserIdQuery request,
        CancellationToken cts)
        {
            if (_context.Appointments != null && _context.CustomerDatas!=null)
            {
                var customerData = await _context.CustomerDatas.FirstOrDefaultAsync(c => c.UserId.Equals(request.UsrId), cts);
                if (customerData == null)
                    return null;
                var list = _context.Appointments
                                 .Include(a => a.DayTimeTable)
                                    .ThenInclude(d => d!.DoctorData)
                                         .ThenInclude(x => x!.Speciality).
                                  Include(a => a.DayTimeTable)
                                    .ThenInclude(d => d!.DoctorData)
                                         .ThenInclude(x => x!.User)
                                 .Where(a => a.CustomerDataId.Equals(customerData.Id))
                                 .Select(a => _mapper.Map<AppointmentDto>(a))
                                 .AsEnumerable();
                return list;
            }
            return null;
        }
    }
}
