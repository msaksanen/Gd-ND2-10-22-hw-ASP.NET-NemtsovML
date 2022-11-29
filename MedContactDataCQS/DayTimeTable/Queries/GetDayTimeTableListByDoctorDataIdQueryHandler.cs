using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Queries;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DayTimeTable.Queries
{
    public class GetDayTimeTableListByDoctorDataIdQueryHandler : IRequestHandler<GetDayTimeTableListByDoctorDataIdQuery, IEnumerable<DayTimeTableDto>?>
    {
        private readonly MedContactContext _context;

        private readonly IMapper _mapper;

        public GetDayTimeTableListByDoctorDataIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DayTimeTableDto>?> Handle(GetDayTimeTableListByDoctorDataIdQuery request,
            CancellationToken cts)
        {
            if (_context.DayTimeTables != null && _context.DayTimeTables.Any())
            {
                var list = await  _context.DayTimeTables
                                  .Where(d => d.DoctorDataId != null && d.DoctorDataId.Equals(request.DoctorDataId))
                                  .Select(d => _mapper.Map<DayTimeTableDto>(d))
                                  .ToListAsync();
                return list;
            }
            return null;
        }
    }
}
