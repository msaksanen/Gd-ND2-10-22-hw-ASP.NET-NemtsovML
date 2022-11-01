using AutoMapper;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactDb.Entities;
using System.Security.Claims;

namespace MedContactApp.Helpers
{
    public class ModelUserBuilder
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public ModelUserBuilder(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        internal async Task<BaseUserModel?> BuildById(string? id, HttpContext context)
        {
            string userId;
            if (string.IsNullOrEmpty(id))
            {
                var UserIdClaim = context.User.FindFirst("MUId");
                userId = UserIdClaim!.Value;
            }
            else
                userId = id;

            var result = Guid.TryParse(userId, out Guid guid_id);

            if (result)
            {
                var usr = await _userService.GetUserByIdAsync(guid_id);
                var baseUserModel = _mapper.Map<BaseUserModel>(usr);
                var roles = context.User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                if (roles != null) baseUserModel.RoleNames = roles;

                return baseUserModel;
            }
            return null;
        }

        internal async Task<BaseUserModel?> BuildById(HttpContext context)
        {          
            var UserIdClaim = context.User.FindFirst("MUId");
            string userId = UserIdClaim!.Value;
   
            var result = Guid.TryParse(userId, out Guid guid_id);

            if (result)
            {
                var usr = await _userService.GetUserByIdAsync(guid_id);
                var baseUserModel = _mapper.Map<BaseUserModel>(usr);
                var roles = context.User.Claims.Select(c => ClaimsIdentity.DefaultRoleClaimType).ToList();
                if (roles != null) baseUserModel.RoleNames = roles;

                return baseUserModel;
            }
            return null;
        }
    }
}
