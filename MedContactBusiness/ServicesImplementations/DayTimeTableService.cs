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
    public class DayTimeTableService: IDayTimeTableService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public DayTimeTableService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateDayTimeTableAsync (DayTimeTableDto dto)
        {
            int addingResult = 0;
            var entity = _mapper.Map<DayTimeTable>(dto);
            if (entity != null && entity.StartWorkTime != null && entity.FinishWorkTime != null && 
                entity.ConsultDuration != null && entity.ConsultDuration >0)
            {
                int startMin= entity.StartWorkTime.Value.Hour * 60 + entity.StartWorkTime.Value.Minute;
                int finishMin = entity.FinishWorkTime.Value.Hour * 60 + entity.FinishWorkTime.Value.Minute;
                entity.TotalTicketQty= (int)Math.Floor((double)((finishMin - startMin) / entity.ConsultDuration));
                entity.FreeTicketQty = entity.TotalTicketQty;
                var noOverLap = await _unitOfWork.DayTimeTableRepository.Get()
                         .Where(d =>d.DoctorDataId.Equals(entity.DoctorDataId))
                         .AllAsync(d => entity.StartWorkTime>=d.FinishWorkTime || d.StartWorkTime>=entity.FinishWorkTime); 
                if (noOverLap)
                {
                    await _unitOfWork.DayTimeTableRepository.AddAsync(entity);
                    addingResult = await _unitOfWork.Commit();
                }
                else
                    addingResult = -1;

                    return addingResult;
            }
            else
            {
                    throw new ArgumentException(nameof(dto));
            }          
        }

        //private  bool isOverLap (DayTimeTable old, DayTimeTable newd)
        //{
        //    if (old.StartWorkTime == newd.StartWorkTime || old.FinishWorkTime == newd.FinishWorkTime)
        //            return true;
        //    if (newd.StartWorkTime >= old.FinishWorkTime || old.StartWorkTime >= newd.FinishWorkTime)
        //            return false;

        //    return true;
        //}
        public async Task<List<DayTimeTableDto>> GetDayTimeTableByPageNumberAndPageSizeAsync(int pageNumber, int pageSize)
        {
           
                var list = await _unitOfWork.DayTimeTableRepository
                                  .Get()
                                  .Skip(pageNumber * pageSize)
                                  .Take(pageSize)
                                  .Select(DayTimeTable => _mapper.Map<DayTimeTableDto>(DayTimeTable))
                                  .ToListAsync();
                return list;
            
        }
        public async Task<int> GetDayTimeTableEntitiesCountAsync()
        {
           
                return await _unitOfWork.DayTimeTableRepository.Get().CountAsync();
        }

        public async Task<IEnumerable<DayTimeTableDto>?> GetDayTimeTableByDoctorDataId(Guid dataId)
        {
            var list = await  _unitOfWork.DayTimeTableRepository.Get()
                                                         .Where(d => d.DoctorDataId != null && d.DoctorDataId.Equals(dataId))
                                                         .Select(d => _mapper.Map<DayTimeTableDto>(d))
                                                         .ToListAsync();
            return list;
        }

    }
}
