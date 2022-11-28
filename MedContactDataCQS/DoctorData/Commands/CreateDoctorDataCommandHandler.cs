using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDb;
using MedContactDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DoctorData.Commands
{
    public class CreateDoctorDataCommandHandler: IRequestHandler<CreateDoctorDataCommand, int>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public CreateDoctorDataCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateDoctorDataCommand command, CancellationToken token)
        {
            int res = 0;
            if (command.DoctorDataDto == null)
                return res;
            if (_context.DoctorDatas != null && _context.DoctorDatas.Any())
            {
                var entity = _mapper.Map<MedContactDb.Entities.DoctorData>(command.DoctorDataDto);
                if  (entity.UserId == null || entity.SpecialityId == null)
                    return res;

                else
                {
                    var dData = await _context.DoctorDatas.FirstOrDefaultAsync
                       (dd => dd.UserId.Equals(entity.UserId) && dd.SpecialityId.Equals(entity.SpecialityId));
                    if (entity != null && dData == null)
                    {
                        await _context.DoctorDatas.AddAsync(entity, token);
                    }
                    else if (entity != null && dData != null && dData.ForDeletion == true)
                    {
                        dData.ForDeletion = false;
                    }

                }
            }
            res = await _context.SaveChangesAsync();
            return res;     
        }
    }
}
