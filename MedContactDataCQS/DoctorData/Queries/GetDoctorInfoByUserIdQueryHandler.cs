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
    public class GetDoctorInfoByUserIdQueryHandler : IRequestHandler<GetDoctorInfoByUserIdQuery, List<DoctorInfo>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetDoctorInfoByUserIdQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DoctorInfo>?> Handle(GetDoctorInfoByUserIdQuery request,
            CancellationToken cts)
        {
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var dDataList = await _context.DoctorDatas
                                 .Include(t => t.Speciality)
                                 .Include(t => t.User)
                                 .Where(t => t.UserId.Equals(request.UserId))
                                 .Where(t => t.Speciality != null)
                                 .ToListAsync(cts);


                if (dDataList.Any())
                {
                    var docInfoList = dDataList.Select(t => _mapper.Map<DoctorInfo>((t.User, t.Speciality, t))).ToList();
                    return docInfoList;
                }
            }
            return null;
        }
    }
}
