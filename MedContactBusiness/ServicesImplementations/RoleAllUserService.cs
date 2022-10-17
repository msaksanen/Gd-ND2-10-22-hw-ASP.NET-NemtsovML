//using AutoMapper;
//using MedContactCore;
//using MedContactCore.Abstractions;
//using MedContactCore.DataTransferObjects;
//using MedContactDataAbstractions;
//using MedContactDataAbstractions.Repositories;
//using MedContactDb.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Security.Principal;

//namespace MedContactBusiness.ServicesImplementations
//{
//    public  class RoleAllUserService<DTO,B> : IRoleAllUserService<DTO>
//            where DTO : BaseUserDto
//            where B : BaseUser
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly IConfiguration _configuration;

//        public RoleAllUserService (IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
//        {

//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _configuration = configuration;
//        }
//        public  async Task<int> RegisterWithRoleAsync(DTO dto)
//        {
//            var entity = _mapper.Map<B>(dto);
//            entity.PasswordHash = CreateMd5(dto.PasswordHash);
//            Guid roleId = (Guid)dto.RoleId!;
    
//            if (typeof(B).Equals(typeof(Customer)))
//            {
//                    Customer? customer = entity as Customer;
//                    await _unitOfWork.RoleAllUserRepository.AddAsync(new() { RoleId = roleId, CustomerId = entity.Id, Customer = customer });
//                    var addingResult = await _unitOfWork.Commit();
//                    return addingResult;
//            }
//            else if (typeof(B).Equals(typeof(Doctor)))
//            {
//                    Doctor? doctor = entity as Doctor;
//                    await _unitOfWork.RoleAllUserRepository.AddAsync(new() { RoleId = roleId, DoctorId = entity.Id, Doctor = doctor });
//                    var addingResult = await _unitOfWork.Commit();
//                    return addingResult;
//            }
//            else if (typeof(B).Equals(typeof(User)))
//            {
//                    User? user = entity as User;
//                    await _unitOfWork.RoleAllUserRepository.AddAsync(new() { RoleId = roleId, UserId = entity.Id, User = user });
//                    var addingResult = await _unitOfWork.Commit();
//                    return addingResult;
//            }
//            else
//            {
//                throw new ArgumentException(nameof(dto));
//            }
//        }

//        private string CreateMd5(string? password)
//        {
//            var passwordSalt = _configuration["UserSecrets:PasswordSalt"];

//            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
//            {
//                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
//                var hashBytes = md5.ComputeHash(inputBytes);

//                return Convert.ToHexString(hashBytes);
//            }
//        }
//    }
//}
