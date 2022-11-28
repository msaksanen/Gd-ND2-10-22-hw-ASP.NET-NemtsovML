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

namespace MedContactDataCQS.DoctorData.Queries
{
    public class GetDoctorDataListByUserIdQueryHandler : IRequestHandler<GetDoctorDataListByUserIdQuery, List<DoctorDataDto>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetDoctorDataListByUserIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;   
        }

        public async Task<List<DoctorDataDto>?> Handle(GetDoctorDataListByUserIdQuery request,
            CancellationToken cts)
        {
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var list = await _context.DoctorDatas
                         .AsNoTracking()
                         .Where(dd => dd.UserId.Equals(request.UserId))
                         .Where(dd => dd.SpecialityId != null)
                         .Select(dd => _mapper.Map<DoctorDataDto>(dd))
                         .ToListAsync();

                return list;
            }
            return null;
        }
    }
}
