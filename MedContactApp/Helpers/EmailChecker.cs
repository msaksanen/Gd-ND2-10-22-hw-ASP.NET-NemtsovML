using MedContactCore.Abstractions;
//using MedContactDb.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedContactCore;

using MedContactCore.DataTransferObjects;
using MedContactApp.Models;
using AutoMapper;
using Serilog;
using System.ComponentModel.Design;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using MedContactBusiness.ServicesImplementations;

using System.Runtime.InteropServices;
using MedContactApp.Helpers;
using Microsoft.CodeAnalysis.Differencing;

namespace MedContactApp.Helpers
{
    public class EmailChecker
    {
        private readonly IUserService _userService;
        private readonly IFamilyService _familyService;
        public EmailChecker(IUserService userService, IFamilyService familyService)
        {
            _userService= userService;
            _familyService = familyService;
        }
        public async Task <bool> CheckEmail(string email, HttpContext context)
        {
            var curEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if  (curEmail != null && curEmail.Equals(email)) 
                return true;

            var mainUserId = context.User.FindFirst("MUId")?.Value;
          
            if (Guid.TryParse(mainUserId, out Guid Uid))
            {
                var family = await _familyService.GetRelativesAsync(Uid);
                if (family != null && family.Any(r => r.Email!=null && r.Email.Equals(email)))
                    return true;
            }

                if (await _userService.CheckUserEmailAsync(email))
                return false;
            return true;
        }
    }
}
