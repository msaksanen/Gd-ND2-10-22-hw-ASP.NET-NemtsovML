using AutoMapper;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataAbstractions;
using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MedContactBusiness.ServicesImplementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateCustomerAsync(CustomerDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            entity.PasswordHash = CreateMd5(dto.PasswordHash);

            if (entity != null)
            {
                await _unitOfWork.CustomerRepository.AddAsync(entity);
                var addingResult = await _unitOfWork.Commit();
                return addingResult;
            }
            else
            {
                throw new ArgumentException(nameof(dto));
            }
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.CustomerRepository.GetByIdAsync(id);
            var dto = _mapper.Map<CustomerDto>(entity);

            return dto;
        }

        public async Task<List<CustomerDto>> GetCustomersByPageNumberAndPageSizeAsync(int pageNumber, int pageSize)
        {
            var list = await _unitOfWork.CustomerRepository
                .Get()
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(customer => _mapper.Map<CustomerDto>(customer))
                .ToListAsync();

            return list;
        }

        public List<CustomerDto> GetNewCustomersFromExternalSources()
        {
            var list = new List<CustomerDto>();
            return list;
        }

        public async Task<int> PatchAsync(Guid id, List<PatchModel> patchList)
        {
            await _unitOfWork.CustomerRepository.PatchAsync(id, patchList);
            return await _unitOfWork.Commit();
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
    }
}