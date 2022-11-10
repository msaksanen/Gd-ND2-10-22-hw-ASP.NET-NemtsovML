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
    public class SpecialityService : ISpecialityService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialityService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SpecialityDto>?> GetSpecialitiesListAsync()
        {
            return await _unitOfWork.SpecialityRepository
                         .Get()
                         .Select(sp => _mapper.Map<SpecialityDto>(sp))
                         .ToListAsync();
        }

        public async Task<int> DeleteSpecialityByName(string? name)
        {
            int res = 0;
            if (name != null)
            {
                var spec = await _unitOfWork.SpecialityRepository.Get().FirstOrDefaultAsync(x => x.Name != null && x.Name.Equals(name));

                if (spec != null) _unitOfWork.SpecialityRepository.Remove(spec);
                res = await _unitOfWork.Commit();
            }
            return res;
        }

        public async Task<int> AddSpecialityToDb(SpecialityDto dto)
        {
            var entity = _mapper.Map<Speciality>(dto);
            if (entity != null && !(_unitOfWork.SpecialityRepository.Get().Any(s => s.Name != null && s.Name.Equals(entity.Name))))
                await _unitOfWork.SpecialityRepository.AddAsync(entity);
            var res = await _unitOfWork.Commit();

            return res;
        }

        public async Task<Result> RemoveSpecialityById(Guid id)
        {
          var entity = await _unitOfWork.SpecialityRepository.GetByIdAsync(id);
          Result removeResult = new ();
          if (entity != null)
          {
                removeResult.Name = entity.Name;
                _unitOfWork.SpecialityRepository.Remove(entity);
                removeResult.IntResult = await  _unitOfWork.Commit();
            }
           return removeResult;
        }

    }
}
