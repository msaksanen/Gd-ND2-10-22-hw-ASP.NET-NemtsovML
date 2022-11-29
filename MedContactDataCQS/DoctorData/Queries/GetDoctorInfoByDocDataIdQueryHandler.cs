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
    public class GetDoctorInfoByDocDataIdQueryHandler : IRequestHandler<GetDoctorInfoByDocDataIdQuery, DoctorInfo?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetDoctorInfoByDocDataIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DoctorInfo?> Handle(GetDoctorInfoByDocDataIdQuery request,
            CancellationToken cts)
        {
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var dData = await _context.DoctorDatas
                           .Where(t => t.Id.Equals(request.DoctorDataId) && t.SpecialityId != null)
                           .Include(t => t.User)
                           .Include(t => t.Speciality)
                           .FirstOrDefaultAsync(cts);

                if (dData?.Speciality != null && dData?.User != null)
                {
                    var doctInfo = _mapper.Map<DoctorInfo>((dData.User, dData.Speciality, dData));
                    return doctInfo;
                }
            }
            return null;
        }
    }
}
