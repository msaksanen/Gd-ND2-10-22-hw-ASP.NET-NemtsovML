using AutoMapper;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Commands
{
    public class MarkForDeleteDoctorDataCommandHandler : IRequestHandler<MarkForDeleteDoctorDataCommand, int>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public MarkForDeleteDoctorDataCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(MarkForDeleteDoctorDataCommand command, CancellationToken token)
        {
            int res = 0;
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var entity = _mapper.Map<MedContactDb.Entities.DoctorData>(command.DoctorDataDto);
                entity.ForDeletion = true;
                _context.DoctorDatas.Update(entity);
            }
            res = await _context.SaveChangesAsync();
            return res;
        }
    }
}



