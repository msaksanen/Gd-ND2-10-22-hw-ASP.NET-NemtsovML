using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MedContactApp.Helpers
{
    public class EmailChecker
    {
        private readonly IUserService _userService;

        public EmailChecker(IUserService userService)
        {
            _userService= userService;
        }
        public async Task <bool> CheckEmail(string email)
        {
            if (await _userService.CheckUserEmailAsync(email))
                return false;
            return true;
        }
    }
}
