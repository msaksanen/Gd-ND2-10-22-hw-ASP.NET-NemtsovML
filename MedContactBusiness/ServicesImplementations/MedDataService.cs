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

        public async Task<int> RemoveMedDataById(Guid id)
        {
            var result = 0;
            var meddata = await _unitOfWork.MedDataRepository.GetByIdTrackAsync(id);

            if (meddata == null)
                return result;
            
            _unitOfWork.MedDataRepository.Remove(meddata);
            result = await _unitOfWork.Commit();

            return result;
        }
    }
}
