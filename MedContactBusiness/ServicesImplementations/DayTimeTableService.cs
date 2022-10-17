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
            var entity = _mapper.Map<DayTimeTable>(dto);
            if (entity != null && entity.StartWorkTime != null && entity.FinishWorkTime != null && 
                entity.ConsultDuration != null && entity.ConsultDuration >0)
            {
                int startMin= entity.StartWorkTime.Value.Hour * 60 + entity.StartWorkTime.Value.Minute;
                int finishMin = entity.FinishWorkTime.Value.Hour * 60 + entity.FinishWorkTime.Value.Minute;
                entity.TotalTicketQty= (int)Math.Floor((double)((finishMin - startMin) / entity.ConsultDuration));
                entity.FreeTicketQty = entity.TotalTicketQty;
                await _unitOfWork.DayTimeTableRepository.AddAsync(entity);
                    var addingResult = await _unitOfWork.Commit();
                    return addingResult;
            }
            else
            {
                    throw new ArgumentException(nameof(dto));
            }          
        }
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

    }
}
