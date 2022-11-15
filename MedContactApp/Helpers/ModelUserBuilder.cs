using AutoMapper;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using System.Security.Claims;

namespace MedContactApp.Helpers
{
    public class ModelUserBuilder
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        public ModelUserBuilder(IUserService userService, IMapper mapper, IRoleService roleService)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
        }

        internal async Task<BaseUserModel?> BuildById(HttpContext context, string? id = "")
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
                if (usr != null)
                {
                    var baseUserModel = _mapper.Map<BaseUserModel>(usr);
                    var roles = context.User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                    if (roles != null) baseUserModel.RoleNames = roles;

                    return baseUserModel;
                }
            }
            return null;
        }

        internal async Task<UserDto?> BuildUserById(HttpContext context, string? id = "", int? flag=1)
        {
            string userId;
            if (string.IsNullOrEmpty(id))
            {
               if (flag == 2)
                {
                    var UserIdClaim = context.User.FindFirst("UId");
                    userId = UserIdClaim!.Value;
                }
               else
                {
                    var UserIdClaim = context.User.FindFirst("MUId");
                    userId = UserIdClaim!.Value;
                }
            }
            else
                userId = id;

            var result = Guid.TryParse(userId, out Guid guid_id);

            if (result)
            {
                var usr = await _userService.GetUserByIdAsync(guid_id);
                if (usr != null)
                {
                    return usr;
                }
            }
            return null;
        }

    }
}
