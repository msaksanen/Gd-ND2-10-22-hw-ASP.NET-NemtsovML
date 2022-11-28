using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Roles.Queries;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Speciality
{
    public class GetSpecialitiesListQueryHandler : IRequestHandler<GetSpecialitiesListQuery, List<SpecialityDto>?>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public GetSpecialitiesListQueryHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SpecialityDto>?> Handle(GetSpecialitiesListQuery request,
            CancellationToken cts)
        {
            if (_context.Specialities != null && _context.Specialities.Any())
            {
                var list =  _context.Specialities
                                 .Select(sp => _mapper.Map<SpecialityDto>(sp))
                                 .ToList();
                await _context.Specialities.CountAsync();

                return list;
            }
            return null;
        }
    }
}
