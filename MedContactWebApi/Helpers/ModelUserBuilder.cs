using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Users.Queries;
using MedContactDb.Entities;
using MedContactWebApi.Models;
using MediatR;
using System.Security.Claims;

namespace MedContactWebApi.Helpers
{
    /// <summary>
    /// ModelUserBuilder
    /// </summary>
    public class ModelUserBuilder
    { 
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        /// <summary>
        /// ModelUserBuilder Ctor
        /// </summary>
        public ModelUserBuilder(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;   
            _mediator = mediator;
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
                var usr = await _mediator.Send(new GetUserByIdQuery() { UserId = guid_id });
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

        internal async Task<UserDto?> BuildUserDtoById(HttpContext context, string? id = "", int? flag = 1)
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
                var usr = await _mediator.Send(new GetUserByIdQuery() { UserId = guid_id });
                if (usr != null)
                {
                    return usr;
                }
            }
            return null;
        }

    }
}
