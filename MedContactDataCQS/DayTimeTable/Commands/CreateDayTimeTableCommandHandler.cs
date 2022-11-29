using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Commands;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.DayTimeTable.Commands
{
    public class CreateDayTimeTableCommandHandler : IRequestHandler<CreateDayTimeTableCommand, Result>
    {
        private readonly MedContactContext _context;
        private readonly IMapper _mapper;

        public CreateDayTimeTableCommandHandler(MedContactContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> Handle(CreateDayTimeTableCommand command, CancellationToken token)
        {
            Result res = new() { IntResult = 0 };
            if (command.dayTimeTableDto == null)
                return res;
            if (_context.DayTimeTables != null)
            {
                var entity = _mapper.Map<MedContactDb.Entities.DayTimeTable>(command.dayTimeTableDto);
                if (entity != null && entity.StartWorkTime != null && entity.FinishWorkTime != null &&
                    entity.ConsultDuration != null && entity.ConsultDuration > 0)
                {
                    int startMin = entity.StartWorkTime.Value.Hour * 60 + entity.StartWorkTime.Value.Minute;
                    int finishMin = entity.FinishWorkTime.Value.Hour * 60 + entity.FinishWorkTime.Value.Minute;
                    entity.TotalTicketQty = (int)Math.Floor((double)((finishMin - startMin) / entity.ConsultDuration));
                    entity.FreeTicketQty = entity.TotalTicketQty;
                    var noOverLap = await _context.DayTimeTables
                             .Where(d => d.DoctorDataId.Equals(entity.DoctorDataId))
                             .AllAsync(d => entity.StartWorkTime >= d.FinishWorkTime || d.StartWorkTime >= entity.FinishWorkTime, token);
                    if (noOverLap)
                    {
                        await _context.DayTimeTables.AddAsync(entity, token);
                        res.IntResult = await _context.SaveChangesAsync(token);
                        res.IntResult1 = entity.TotalTicketQty;
                        res.IntResult2 = entity.FreeTicketQty;
                    }
                    else
                        res.IntResult = -1;
                }
            }
            return res;
        }
    }
}
