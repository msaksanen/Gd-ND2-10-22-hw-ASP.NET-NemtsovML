using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Tokens.Commands;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using MedContactCore.Abstractions;
using MedContactDataCQS.Users.Commands;
using MedContactWebApi.Models;

namespace MedContactWebApi.Helpers
{
    /// <summary>
    /// JWTSha256 Helper
    /// </summary>
    public class JWTSha256 
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        /// <summary>
        /// JWTSha256 Ctor
        /// </summary>
        public JWTSha256(IConfiguration configuration,
            IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        internal async Task<TokenResponse?> GenerateTokenAsync(UserDto dto, IEnumerable<RoleDto> roleList, string MUId = "")
        {
            return await GenTokenAsync(dto, roleList, MUId);  
        }
        private async Task<TokenResponse?> GenTokenAsync(UserDto dto, IEnumerable<RoleDto> roleList, string MUId="")
        {
            string isfullBlocked = "false";
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:JwtSecret"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var nowUtc = DateTime.UtcNow;
            var exp = nowUtc.AddMinutes(double.Parse(_configuration["Token:ExpiryMinutes"]))
                .ToUniversalTime();
            if (dto.Email != null && roleList != null)
            {
               if (dto.IsFullBlocked == true)
                    isfullBlocked = "true";

               string id = dto.Id.ToString();
               if (MUId.Equals(string.Empty))
                    MUId = id;

               var claims = new List<Claim>()
               {
                 new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")), //jwt unique id from spec
                 new Claim(ClaimTypes.NameIdentifier, dto.Id.ToString("D")),
                 new Claim(ClaimTypes.Email, dto.Email),
                 new Claim("MUId",MUId),
                 new Claim("UId",id),
                 new Claim("FullBlocked", isfullBlocked),
                 //new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Username)
               };

                foreach (var role in roleList)
                    if (role.Name != null)
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));

               var jwtToken = new JwtSecurityToken(_configuration["Token:Issuer"],
               _configuration["Token:Issuer"],
               claims,
               expires: exp,
               signingCredentials: credentials);

               var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

               var refreshTokenValue = Guid.NewGuid();

               await _mediator.Send(new AddRefreshTokenCommand()
               {
                    UserId = dto.Id,
                    TokenValue = refreshTokenValue
               });

                await _mediator.Send(new ChangeUserStatusByIdCommand() { UserId = dto.Id, State = 0 });

                return new TokenResponse()
                {
                    AccessToken = accessToken,
                    Roles = roleList.Select(r =>r.Name).ToArray(),
                    TokenExpiration = jwtToken.ValidTo,
                    UserId = dto.Id,
                    RefreshToken = refreshTokenValue
                };
            }

            return null;
        }

        internal async Task RemoveRefreshTokenAsync(Guid requestRefreshToken, UserDto dto)
        {
             await DeleteRefreshTokenAsync(requestRefreshToken, dto);
        }
        private async Task DeleteRefreshTokenAsync(Guid requestRefreshToken, UserDto dto)
        {
            await _mediator.Send(new ChangeUserStatusByIdCommand() { UserId = dto.Id, State = 1 });

            await _mediator.Send(new RemoveRefreshTokenCommand() {TokenValue = requestRefreshToken });           
        }

    }
}
