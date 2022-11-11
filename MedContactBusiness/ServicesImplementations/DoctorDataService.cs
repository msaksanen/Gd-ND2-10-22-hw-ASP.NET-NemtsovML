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
using System.Xml.Linq;

namespace MedContactBusiness.ServicesImplementations
{
    public class DoctorDataService: IDoctorDataService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorDataService (IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<DoctorInfo> GetDoctorInfoById(Guid? doctorDataId)
        {
            var dData = await _unitOfWork.DoctorDataRepository
                .FindBy(t => t.Id.Equals(doctorDataId) && t.ForDeletion != true && t.SpecialityId!=null ,t => t.User!, t => t.Speciality!)
                .FirstOrDefaultAsync();
           
            if (dData?.Speciality != null && dData?.User != null)
            {
                var doctInfo= _mapper.Map<DoctorInfo>((dData.User, dData.Speciality, dData));
                return doctInfo;
            }
            else
            {
                throw new ArgumentException(nameof(dData));
            }
        }

        public async Task<List<DoctorInfo>?> GetDoctorInfoByUserId(Guid? userId)
        {
            var dDataList = await _unitOfWork.DoctorDataRepository
                                  .Get()
                                  .Include(t => t.Speciality)
                                  .Include(t => t.User)
                                  .Where(t => t.UserId.Equals(userId))
                                  .Where(t => t.Speciality != null)
                                  .ToListAsync();


            if (dDataList.Any())
            {
                var docInfoList = dDataList.Select(t => _mapper.Map<DoctorInfo>((t.User, t.Speciality,t))).ToList();
                return docInfoList;
            }
           
                return null;
        }

        public IQueryable<DoctorData> GetDoctorData()
        {
            return _unitOfWork.DoctorDataRepository.Get();
        }

        public async Task<int> CreateDoctorDataAsync(DoctorDataDto dto)
        {
            var entity = _mapper.Map<DoctorData>(dto);
            int result = 0;
            var dData = await GetDoctorDataByUserIdSpecId(entity.UserId, entity.SpecialityId);

            if (entity != null &&  dData == null)
            {
                await _unitOfWork.DoctorDataRepository.AddAsync(entity);
                result = await _unitOfWork.Commit();   
            }
            else if (entity != null && dData != null && dData.ForDeletion == true)
            {
                dData.ForDeletion = false;
                result = await _unitOfWork.Commit();
            }
            return result;
        }

        public async Task<int> MarkForDeleteDoctorDataAsync(DoctorDataDto dto) 
        {
            var entity = _mapper.Map<DoctorData>(dto);
            entity.ForDeletion = true;
            _unitOfWork.DoctorDataRepository.Update(entity);
            var result = await _unitOfWork.Commit();
            return result;
        }

        public async Task<DoctorData?> GetDoctorDataByUserIdSpecId(Guid? userId, Guid? specId)
        {
            if (userId == null && specId == null)
                return null;

            else
                return await _unitOfWork.DoctorDataRepository.Get().FirstOrDefaultAsync
                    (dd => dd.UserId.Equals(userId) && dd.SpecialityId.Equals(specId));
           
        }
                 

        public async Task<List<DoctorDataDto>> GetDoctorDataListByUserId(Guid userId)
        {
             var list= await _unitOfWork.DoctorDataRepository
                    .Get()
                    .AsNoTracking()
                    //.Include(dd => dd.Speciality)
                    .Where(dd => dd.UserId.Equals(userId))
                    .Where(dd => dd.SpecialityId!=null)
                    .Select(dd => _mapper.Map<DoctorDataDto>(dd))
                    .ToListAsync();


            return list;
        }

        public async Task <int> UpdateDoctorDataAsync(DoctorDataDto dto)
        {
            var data = _mapper.Map<DoctorData>(dto);
            _unitOfWork.DoctorDataRepository.Update(data);
            var res = await _unitOfWork.Commit();
            return res;
        }

        public async Task<int> RemoveByIdAsync(Guid id)
        {
            var data = await _unitOfWork.DoctorDataRepository.GetByIdAsync(id); 
            if (data!=null)
            {
                _unitOfWork.DoctorDataRepository.Remove(data);
            }
            var res = await _unitOfWork.Commit();
            return res;
        }                 
    }
}
