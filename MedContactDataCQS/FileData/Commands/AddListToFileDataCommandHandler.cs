using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Roles.Commands;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.FileData.Commands
{
    public class AddListToFileDataCommandHandler : IRequestHandler<AddListToFileDataCommand, int>
    {
        private readonly MedContactContext _context;
    private readonly IMapper _mapper;

    public AddListToFileDataCommandHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<int> Handle(AddListToFileDataCommand cmd, CancellationToken cts)
    {
        int result = 0;
        if (_context.FileDatas != null && _context.FileDatas.Any() && cmd.FileList!=null)
        {
                List<MedContactDb.Entities.FileData> entityList = new();

                foreach (var item in cmd.FileList)
                    entityList.Add(_mapper.Map<MedContactDb.Entities.FileData>(item));

                await _context.FileDatas.AddRangeAsync(entityList);
                result = await _context.SaveChangesAsync(cts);
            }
        return result;
    }
}

}
