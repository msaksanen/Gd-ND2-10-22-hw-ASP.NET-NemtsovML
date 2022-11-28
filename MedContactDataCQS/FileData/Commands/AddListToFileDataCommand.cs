using MedContactCore.DataTransferObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDataCQS.FileData.Commands
{
    public  class AddListToFileDataCommand : IRequest<int>
    {
        public List<FileDataDto>? FileList { get; set; }
    }
}
