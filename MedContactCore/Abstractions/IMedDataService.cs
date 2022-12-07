using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.Abstractions
{
    public interface IMedDataService
    {
        IQueryable<MedData> GetMedData();
        Task<int> AddMedDataInDbAsync(MedDataDto? dto);
        Task<MedDataDto?> GetMedDataByIdAsync(Guid id);
        Task<int> PatchAsync(Guid id, List<PatchModel> patchList);
        //Task<int> AddListToFileData(List<FileDataDto> list);
        //Task<List<FileDataDto>?> FileDataTolistByUserId(Guid id);
        //Task<int> RemoveFileDataWithFileById(Guid id, string webRootPath);
    }
}
