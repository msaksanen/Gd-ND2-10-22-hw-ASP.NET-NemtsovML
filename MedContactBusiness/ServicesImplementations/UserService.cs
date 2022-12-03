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
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;

namespace MedContactBusiness.ServicesImplementations
{
    public class UserService : IUserService 
 
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateUserAsync(UserDto dto)
        {
            var entity = _mapper.Map<User>(dto);
            entity.PasswordHash = CreateMd5(dto.PasswordHash);
                if (entity!= null)
                {
                    await _unitOfWork.UserRepository.AddAsync(entity);
                    entity.Roles?.Add(new());    
                    var addingResult = await _unitOfWork.Commit();
                    return addingResult;
                }
                else
                {
                    throw new ArgumentException(nameof(dto));
                }
        
        }

        public async Task<int> CreateUserWithRoleAsync(UserDto dto, string roleName)
        {
            var entity = _mapper.Map<User>(dto);
            entity.IsFullBlocked = false;
            var role = await _unitOfWork.RoleRepository
                       .Get()
                       .FirstOrDefaultAsync(role => role.Name != null && role.Name.Equals(roleName));

            entity.PasswordHash = CreateMd5(dto.PasswordHash);

            if (entity != null && role != null)
            {
                entity.Roles?.Add(role);
                await _unitOfWork.UserRepository.AddAsync(entity);
                var addingResult = await _unitOfWork.Commit();
                return addingResult;
            }
            else
            {
                throw new ArgumentException(nameof(dto));
            }

        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserRepository.GetByIdAsync(id); 
            var dto = _mapper.Map<UserDto>(entity);
            var custData = await _unitOfWork.CustomerDataRepository.Get().FirstOrDefaultAsync(c => c.UserId.Equals(id));
            if (custData != null )
                dto.CustomerDataId = custData.Id;
                return dto;
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
                var entity = await _unitOfWork.UserRepository.Get().FirstOrDefaultAsync
                           (user => user.Email != null && user.Email.Equals(email));
                var dto = _mapper.Map<UserDto>(entity);
                return dto;
            
        }

        public async Task<UserDto> GetUserByIdEmailAsync(string email, Guid id)
        {
            var entity = await _unitOfWork.UserRepository.Get().FirstOrDefaultAsync
                       (user => user.Email != null && user.Email.Equals(email) && user.Id.Equals(id));
            var dto = _mapper.Map<UserDto>(entity);
            return dto;

        }

        public async Task<int> GetUserEntitiesCountAsync()
        {
                return await _unitOfWork.UserRepository.Get().CountAsync();
        }

        public async Task<List<UserDto>> GetUsersByPageNumberAndPageSizeAsync(int pageNumber, int pageSize)
        {
            
                var list = await _unitOfWork.UserRepository
                                  .Get()
                                  .Skip(pageNumber * pageSize)
                                  .Take(pageSize)
                                  .Select(customer => _mapper.Map<UserDto>(customer))
                                  .ToListAsync();
     
                return list;       
        }

        public IQueryable<User> GetUsers()
        {
            return _unitOfWork.UserRepository.Get();    
        }


        public List<UserDto> GetNewUsersFromExternalSources()
        {
            var list = new List<UserDto>();
            return list;
        }

        public async Task<bool> CheckUserEmailAsync(string email)
        {
               return await _unitOfWork.UserRepository.Get().AnyAsync
                       (user => user.Email != null && user.Email.Equals(email));
        }


        public async Task<int> PatchAsync(Guid id, List<PatchModel> patchList)
        {
                await _unitOfWork.UserRepository.PatchAsync(id, patchList);
                return await _unitOfWork.Commit();
        }

        public async Task<int> ChangeUserStatusById(Guid id, int state)
        {
            var entity = await _unitOfWork.UserRepository.GetByIdTrackAsync(id);
            if (entity == null) return -1;
            if (state==1) //user is online and logs out
            {
                entity.IsOnline = false;
            }

            else  //user is offline and logs in
            {
                entity.IsOnline = true;
                entity.LastLogin = DateTime.Now;
            }

            var result= await _unitOfWork.Commit();
            return result;
        }

        public async Task<int> ChangeUserFullBlockById(Guid id, bool? IsFullBlocked)
        {
            var entity = await _unitOfWork.UserRepository.GetByIdTrackAsync(id);
            if (entity == null) return 0;
            if (IsFullBlocked==true)
                entity.IsFullBlocked = true;
            else
                entity.IsFullBlocked = false;
            var result = await _unitOfWork.Commit();
            return result;

        }

        private string CreateMd5(string? password)
        {
            var passwordSalt = _configuration["UserSecrets:PasswordSalt"];

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        public async Task<bool> IsUserExists(Guid userId)
        {
                return await _unitOfWork.UserRepository.Get().AnyAsync(user => user.Id.Equals(userId));
        }

        public async Task<bool> CheckUserPassword(string email, string password)
        {
                var dbPasswordHash = (await _unitOfWork.UserRepository.Get()
                .FirstOrDefaultAsync(user => user.Email!=null && user.Email.Equals(email))) ?.PasswordHash;

                return
                    dbPasswordHash != null
                    && CreateMd5(password).Equals(dbPasswordHash);
            
        }

        public async Task<bool> CheckUserPassword(Guid userId, string password)
        {
                var dbPasswordHash = (await _unitOfWork.UserRepository.Get()
                .FirstOrDefaultAsync(user => user.Id.Equals(userId)))?.PasswordHash;

                return
                    dbPasswordHash != null
                    && CreateMd5(password).Equals(dbPasswordHash);
        }
        
        public async Task<int> ChangeUserPasswordAsync (Guid id, string password)
        {
            var sourceUser = await _unitOfWork.UserRepository.GetByIdTrackAsync(id);
            if (sourceUser != null)
            {
                string pwdHash = CreateMd5(password);
                sourceUser.PasswordHash = pwdHash;
                var addingResult = await _unitOfWork.Commit();
                return addingResult;
            }
            else
            {
                throw new ArgumentException(nameof(password));
            }
        }
        public async Task<Result> GetIdByEmailUserPassword(string email, string password)
        {
            Result res = new() { IntResult = 0 };

            string pwdHash = CreateMd5(password);
            var user = await _unitOfWork.UserRepository.Get()
                        .FirstOrDefaultAsync(u => u.IsDependent != true
                        && u.Email != null && u.Email.Equals(email)
                        && u.PasswordHash != null && u.PasswordHash.Equals(pwdHash));
            if (user != null)
            {
                res.GuidResult = user.Id;
                res.IntResult = 2; 
                return res;
            }

            var emptyHash = CreateMd5("");
            var userResetPwd = await _unitOfWork.UserRepository.Get()
                        .FirstOrDefaultAsync(u => u.IsDependent != true
                        && u.Email != null && u.Email.Equals(email)
                        && u.PasswordHash != null && u.PasswordHash.Equals(emptyHash));
            if (userResetPwd != null)
            {
                res.GuidResult = userResetPwd.Id;
                res.IntResult = 1;
                return res;
            }
            else
                return res;
        }
        public async Task<int> CheckUserByUsernamePassword(Guid id, string pwd, string username)
        {
            var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);

            if (entity == null)
                return 0;
            if (entity.PasswordHash == CreateMd5(pwd) && entity.Username == username)
                return 2;

            return 1;
        }

        public async Task<int> RemoveUserById (Guid id)
        {
            int res = 0;
            var entity = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (entity!=null)
            {
                _unitOfWork.UserRepository.Remove(entity);
                res = await _unitOfWork.Commit();
            }
           
            return res;
        }
    }
}