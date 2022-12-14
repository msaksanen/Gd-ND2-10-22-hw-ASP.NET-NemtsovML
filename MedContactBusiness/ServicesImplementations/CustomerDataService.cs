﻿using AutoMapper;
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
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactBusiness.ServicesImplementations
{
    public class CustomerDataService: ICustomerDataService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerDataService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        //public async Task<Result> CreateByUserIdAsync(Guid userId)
        //{
        //   var res = new Result() { IntResult = 0 };
        //   var ent =new CustomerData() { Id = Guid.NewGuid(), IsBlocked =false, UserId = userId };
        //   //var usr = await _unitOfWork.UserRepository.GetByIdTrackAsync(userId); 
        //   await _unitOfWork.CustomerDataRepository.AddAsync(ent);
        //    //if (usr != null)
        //    //    usr.CustomerData = ent;
        //    res.IntResult = await _unitOfWork.Commit();
        //   if (res.IntResult > 0)
        //        res.GuidResult = ent.Id;
        //   return res;
        //}

        public async Task<CustomerDataDto?> CreateByUserIdAsync(Guid userId)
        {
            var ent = await _unitOfWork.CustomerDataRepository.Get().FirstOrDefaultAsync(c => c.UserId.Equals(userId));
            if (ent != null)
            {
                return _mapper.Map<CustomerDataDto>(ent);   
            }
            else
            {
                var newEnt = new CustomerData() { Id = Guid.NewGuid(), IsBlocked = false, UserId = userId};
                await _unitOfWork.CustomerDataRepository.AddAsync(newEnt);
                var res = await _unitOfWork.Commit();

                if (res > 0)
                    return _mapper.Map<CustomerDataDto>(newEnt);
            }
            return null;
        }

        public async Task<CustomerDataDto?> GetByIdAsync(Guid customerDataId)
        {
            var cd = await _unitOfWork.CustomerDataRepository.GetByIdAsync(customerDataId);
            if (cd == null)
                return null;

            return _mapper.Map<CustomerDataDto>(cd);
        }
    }
}
