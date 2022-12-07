using AutoMapper;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;
using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Principal;

namespace MedContactBusiness.ServicesImplementations
{
    public class MedDataService: IMedDataService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public MedDataService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public IQueryable<MedData> GetMedData()
        {
            return _unitOfWork.MedDataRepository.Get();
        }


        public async Task<int> AddMedDataInDbAsync(MedDataDto? dto)
        {
            var entity = _mapper.Map<MedData>(dto); 
            await _unitOfWork.MedDataRepository.AddAsync(entity);
            var addingResult = await _unitOfWork.Commit();
            return addingResult;
        }

        public async Task<MedDataDto?> GetMedDataByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.MedDataRepository.GetByIdAsync(id);
            if (entity == null)
                return null;
            else
                return _mapper.Map<MedDataDto>(entity);
        }

        public async Task<int> PatchAsync(Guid id, List<PatchModel> patchList)
        {
            await _unitOfWork.MedDataRepository.PatchAsync(id, patchList);
            return await _unitOfWork.Commit();
        }

        //public async Task<int> AddListToFileData(List<FileDataDto> list)
        //{
        //    List <FileData> entityList = new();
        //    foreach (var item in list)
        //        entityList.Add(_mapper.Map<FileData>(item));

        //    await _unitOfWork.FileDataRepository.AddRangeAsync(entityList);
        //    var addingResult = await _unitOfWork.Commit();
        //    return addingResult;
        //}

        //public async Task<List<FileDataDto>?> FileDataTolistByUserId(Guid id)
        //{
        //  var list =await  _unitOfWork.FileDataRepository.Get()
        //                   .Where(x => x.UserId.Equals(id))
        //                   .Select(x => _mapper.Map<FileDataDto>(x))
        //                   .ToListAsync();

        //  return list;
        //}

        //public async Task<int> RemoveFileDataWithFileById(Guid id, string webRootPath)
        //{
        //    var result = -2;
        //    var filedata = await _unitOfWork.FileDataRepository.GetByIdTrackAsync(id);

        //    if (filedata == null)
        //        return result;

        //    if (filedata.Path == null)
        //    {
        //         _unitOfWork.FileDataRepository.Remove(filedata);
        //        result += await _unitOfWork.Commit();
        //        return result;
        //    }

        //    string path = Path.Combine(webRootPath, filedata.Path!);

        //    if (File.Exists(path))
        //    {
        //        File.Delete(path);
        //        _unitOfWork.FileDataRepository.Remove(filedata);
        //        result = 1 + await _unitOfWork.Commit();
        //    }
        //    else 
        //    {
        //        _unitOfWork.FileDataRepository.Remove(filedata);
        //        result = await _unitOfWork.Commit();
        //    }

        //    return result;
        //}
    }
}
