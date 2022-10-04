using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MedContactApp.Helpers
{
    public class EmailChecker<DTO> where DTO : BaseUserDto
    {
        private readonly IBaseUserService<DTO> _baseUserService;

        public EmailChecker(IBaseUserService<DTO> baseUserService)
        {
            _baseUserService=baseUserService;
        }
        public async Task <bool> CheckEmail(string email)
        {
            if (await _baseUserService.CheckBaseUserEmailAsync(email))
                return false;
            return true;
        }
    }
}
