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
using System.Data;
using System.Security.Principal;

namespace MedContactBusiness.ServicesImplementations
{
    public class FamilyService : IFamilyService
 
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public FamilyService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<UserDto>?> GetRelativesAsync(Guid mainUId)
        {

            var family = await _unitOfWork.FamilyRepository
                        .Get()
                        .FirstOrDefaultAsync(f => f.MainUserId.Equals(mainUId));

            if (family != null)
            {
                var list = await _unitOfWork.UserRepository
                               .Get()
                               .Where(u => u.FamilyId.Equals(family!.Id))
                               .Select(customer => _mapper.Map<UserDto>(customer))
                               .ToListAsync();

                return list;
            }

            return null;
        }
        public async Task<int> CreateNewFamilyForMainUser (UserDto mainUserDto, FamilyDto familyDto)
        {
            var mainUser = _mapper.Map<User>(mainUserDto);
            var family = _mapper.Map<Family>(familyDto);

            _unitOfWork.UserRepository.Update(mainUser);
            await _unitOfWork.FamilyRepository.AddAsync(family);
            var addingResult = await _unitOfWork.Commit();

            return addingResult;
        }
    }
}