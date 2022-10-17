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
//    public class BaseUserService<DTO,B> : IBaseUserService<DTO> 
//                 where DTO : BaseUserDto
//                 where B: BaseUser
//    {
//        private readonly IMapper _mapper;
//        private readonly IConfiguration _configuration;
//        private readonly IUnitOfWork _unitOfWork;
//        private IBaseUserRepository<B>? _unitOfWorkRepos;

//        public BaseUserService(IMapper mapper,
//        IConfiguration configuration,
//        IUnitOfWork unitOfWork)
//        {
//            _mapper = mapper;
//            _configuration = configuration;
//            _unitOfWork = unitOfWork;
//            SetRepository();
//        }
//        public async Task<int> CreateBaseUserAsync(DTO dto)
//        {
//            var entity = _mapper.Map<B>(dto);
//            entity.PasswordHash = CreateMd5(dto.PasswordHash);
//            if (_unitOfWorkRepos != null)
//            {
//                if (entity!= null)
//                {
//                    await _unitOfWorkRepos.AddAsync(entity);
//                    var addingResult = await _unitOfWork.Commit();
//                    return addingResult;
//                }
//                else
//                {
//                    throw new ArgumentException(nameof(dto));
//                }
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        public async Task<DTO> GetBaseUserByIdAsync(Guid id)
//        {
//           if (_unitOfWorkRepos != null)
//            {
//                var entity = await _unitOfWorkRepos.GetByIdAsync(id);
//                var dto = _mapper.Map<DTO>(entity);
//                return dto;
//            }
//            else
//            {
//                throw new NullReferenceException (nameof(_unitOfWorkRepos));
//            }

//        }

//        public async Task<DTO> GetBaseUserByEmailAsync(string email)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                var entity = await _unitOfWorkRepos.Get().AnyAsync
//                           (user => user.Email != null && user.Email.Equals(email));
//                var dto = _mapper.Map<DTO>(entity);
//                return dto;
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        public async Task<int> GetBaseUserEntitiesCountAsync()
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                return await _unitOfWorkRepos.Get().CountAsync();
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        public async Task<List<DTO>> GetBaseUsersByPageNumberAndPageSizeAsync(int pageNumber, int pageSize)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                var list = await _unitOfWorkRepos
//                                  .Get()
//                                  .Skip(pageNumber * pageSize)
//                                  .Take(pageSize)
//                                  .Select(customer => _mapper.Map<DTO>(customer))
//                                  .ToListAsync();

//                return list;
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        public List<DTO> GetNewBaseUsersFromExternalSources()
//        {
//            var list = new List<DTO>();
//            return list;
//        }

//        public async Task<bool> CheckBaseUserEmailAsync(string email)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                return await _unitOfWorkRepos.Get().AnyAsync
//                       (user => user.Email != null && user.Email.Equals(email));
//            }       
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }


//        public async Task<int> PatchAsync(Guid id, List<PatchModel> patchList)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                await _unitOfWorkRepos.PatchAsync(id, patchList);
//                return await _unitOfWork.Commit();
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
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

       

//        public async Task<bool> IsBaseUserExists(Guid userId)
//        {
//            if (_unitOfWorkRepos != null)
//                return await _unitOfWorkRepos.Get().AnyAsync(user => user.Id.Equals(userId));
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        public async Task<bool> CheckBaseUserPassword(string email, string password)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                var dbPasswordHash = (await _unitOfWorkRepos.Get()
//                .FirstOrDefaultAsync(user => user.Email!=null && user.Email.Equals(email))) ?.PasswordHash;

//                return
//                    dbPasswordHash != null
//                    && CreateMd5(password).Equals(dbPasswordHash);
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }

//        }

//        public async Task<bool> CheckBaseUserPassword(Guid userId, string password)
//        {
//            if (_unitOfWorkRepos != null)
//            {
//                var dbPasswordHash = (await _unitOfWorkRepos.Get()
//                .FirstOrDefaultAsync(user => user.Id.Equals(userId)))?.PasswordHash;

//                return
//                    dbPasswordHash != null
//                    && CreateMd5(password).Equals(dbPasswordHash);
//            }
//            else
//            {
//                throw new NullReferenceException(nameof(_unitOfWorkRepos));
//            }
//        }

//        private void SetRepository()
//        {
//            if (typeof(B).Equals(typeof(Customer))) _unitOfWorkRepos = (IBaseUserRepository<B>)_unitOfWork.CustomerRepository;
//            if (typeof(B).Equals(typeof(Doctor))) _unitOfWorkRepos = (IBaseUserRepository<B>)_unitOfWork.DoctorRepository;
//        }
//    }
//}