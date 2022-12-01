using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Queries
{
    public class GetCDataByIdQueryHandler : IRequestHandler<GetCDataByIdQuery, CustomerDataDto?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

    public GetCDataByIdQueryHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CustomerDataDto?> Handle(GetCDataByIdQuery request,
    CancellationToken cts)

    {
        if (_context.CustomerDatas != null && _context.CustomerDatas.Any())
        {
                var entity = await _context.CustomerDatas.AsNoTracking()
                             .FirstOrDefaultAsync(entity => entity.Id.Equals(request.CustomerDataId),cts);

                if (entity == null) return null;

                var dto = _mapper.Map<CustomerDataDto>(entity);
                return dto;
        }
        return null;
    }
}
}
