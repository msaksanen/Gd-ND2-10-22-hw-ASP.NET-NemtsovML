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
                .FindBy(t => t.Id.Equals(doctorDataId),t => t.User!, t => t.Speciality!)
                .FirstOrDefaultAsync();
           
            if (dData?.Speciality != null && dData?.User != null)
            {
                var doctInfo= _mapper.Map<DoctorInfo>((dData.User, dData.Speciality));
                return doctInfo;
            }
            else
            {
                throw new ArgumentException(nameof(dData));
            }
        }
    }
}
