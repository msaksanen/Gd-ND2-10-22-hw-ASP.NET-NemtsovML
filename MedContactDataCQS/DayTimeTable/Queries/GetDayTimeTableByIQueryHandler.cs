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

namespace MedContactDataCQS.DayTimeTable.Queries
{
    public class GetDayTimeTableByIdQueryHandler : IRequestHandler<GetDayTimeTableByIdQuery, DayTimeTableDto?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

    public GetDayTimeTableByIdQueryHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DayTimeTableDto?> Handle(GetDayTimeTableByIdQuery request,
    CancellationToken cts)

    {
        if (_context.DayTimeTables != null && _context.DayTimeTables.Any())
        {
                var entity = await _context.DayTimeTables.AsNoTracking()
                             .FirstOrDefaultAsync(entity => entity.Id.Equals(request.DttId),cts);

                if (entity == null) return null;

                var dto = _mapper.Map<DayTimeTableDto>(entity);
                return dto;
        }
        return null;
    }
}
}
