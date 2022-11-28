using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Queries;
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
    public class GetDoctorDataQueryHandler : IRequestHandler<GetDoctorDataQuery, IQueryable<MedContactDb.Entities.DoctorData>?>
    {
        private readonly MedContactContext _context;

        public GetDoctorDataQueryHandler(MedContactContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<MedContactDb.Entities.DoctorData>?> Handle(GetDoctorDataQuery request,
            CancellationToken cts)
        {
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var list = _context.DoctorDatas.Include(d => d.Speciality)
                                               .Include(d => d.User)
                                               .ThenInclude(u => u!.Roles)
                                               .AsQueryable();

                await _context.DoctorDatas.CountAsync();

                return list;
            }
            return null;
        }
    }
}
