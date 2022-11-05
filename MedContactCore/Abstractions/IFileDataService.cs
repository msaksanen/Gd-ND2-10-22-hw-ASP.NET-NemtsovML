using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IFileDataService
    {
        Task<int> AddListToFileData(List<FileDataDto> list);
        Task<List<FileDataDto>?> FileDataTolistByUserId(Guid id);
        Task<int> RemoveFileDataWithFileById(Guid id, string webRootPath);
    }
}
