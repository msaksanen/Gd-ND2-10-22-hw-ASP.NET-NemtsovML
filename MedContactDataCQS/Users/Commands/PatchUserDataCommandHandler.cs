using AutoMapper;
using MedContactDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.Users.Commands
{
    public class PatchUserDataCommandHandler: IRequestHandler<PatchUserDataCommand, int?>
    {
        private readonly MedContactContext _context;
    private readonly IMapper _mapper;

    public PatchUserDataCommandHandler(MedContactContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int?> Handle(PatchUserDataCommand cmd, CancellationToken cts)
    {
        if (_context.Users != null && _context.Users.Any() && cmd.PatchList!= null && cmd.PatchList.Any())
        {
                var model = await _context.Users.FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.UserId));
                if (model != null)
                {
                    Dictionary<string, object?> nameValuePropertiesPairs = new Dictionary<string, object?>();
                    foreach (var patch in cmd.PatchList)
                    {
                        if (patch.PropertyName != null)
                            nameValuePropertiesPairs.Add(patch.PropertyName, patch.PropertyValue);
                    }

                    var dbEntityEntry = _context.Entry(model);
                    dbEntityEntry.CurrentValues.SetValues(nameValuePropertiesPairs);
                    dbEntityEntry.State = EntityState.Modified;
                }
        }
        var res = await _context.SaveChangesAsync(cts);
        return res;
    }
}
}
