using AutoMapper;
using Hangfire;
using MedContactCore.Abstractions;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Queries;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// Token Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private int _expHours = 6;

        /// <summary>
        /// Token Controller Ctor
        /// </summary>
        public TokenController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, IConfiguration configuration)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
            _configuration = configuration;
        }


        /// <summary>
        /// RemoveOldRefreshTokens
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RemoveOldRefreshTokens()
        {
            bool result = int.TryParse(_configuration["RefreshToken:ExpiryHours"], out var exp);
            if (result) _expHours = exp;
            //var res = await _jwtUtil.RemoveOldRefreshToken(_expHours);
            RecurringJob.AddOrUpdate( () =>  _jwtUtil.RemoveOldRefreshToken(_expHours), "10,40 0-23 * * *");
            return Ok();
        }


        /// <summary>
        /// Create JwtToken (Login)
        /// </summary>
        /// <param name="request LoginUserRequestModel"></param>
        /// <returns>ErrorModel || TokenResponse</returns>
        [HttpPost]
        public async Task<IActionResult> CreateJwtToken([FromBody] LoginUserRequestModel request)
        {
            if (request == null)
                return BadRequest( new {request, Message = "Data is null"});
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest( new {request, Message = "Email is null or empty"});
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest( new {request, Message = "Password is null or empty" });

            try
            {
                var user = await _mediator.Send(new GetUserDtoByEmailPwdHashQuery()
                                {Email = request.Email, PasswordHash = _md5.CreateMd5(request.Password)});
                if (user == null)
                    return BadRequest(new { request, Message = "Password or email are incorrect"});

                var roleList = await _mediator.Send(new GetRoleListByUserIdQuery() {UserId = user.Id });

                if (roleList == null || roleList.Count()==0)
                    return BadRequest(new { request, Message = "User's roles are not found" });


                var response = await _jwtUtil.GenerateTokenAsync(user, roleList);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Refresh JwtToken
        /// </summary>
        /// <param name="request RefreshTokenRequestModel"></param>
        /// <returns>ErrorModel || TokenResponse</returns>
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {

            if (request == null)
                return BadRequest(new { request, Message = "Data is null" });
            if (request?.RefreshToken== null || request.RefreshToken == Guid.Empty)
                return BadRequest(new { request, Message = "Token is null or empty" });

            try
            {
                var user = await _mediator.Send(new GetUserDtoByRefreshTokenQuery()
                                 {RefreshToken = request.RefreshToken });
                if (user == null)
                    return BadRequest(new { request, Message = "Refresh Token is incorrect" });

                var roleList = await _mediator.Send(new GetRoleListByUserIdQuery() { UserId = user.Id });

                if (roleList == null || !roleList.Any())
                    return BadRequest(new { request, Message = "User's roles are not found" });

                var response = await _jwtUtil.GenerateTokenAsync(user, roleList);

                await _jwtUtil.RemoveRefreshTokenAsync(user.Id, request.RefreshToken);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Revoke Token Logout
        /// </summary>
        /// <param name="request RefreshTokenRequestModel"></param>
        /// <returns>StatusCode</returns>
        [HttpPost]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestModel request)
        {
            if (request == null)
                return BadRequest(new { request, Message = "Data is null" });
            if (request?.RefreshToken == null || request.RefreshToken == Guid.Empty)
                return BadRequest(new { request, Message = "Token is null or empty" });
            try
            {
                var user = await _mediator.Send(new GetUserDtoByRefreshTokenQuery()
                { RefreshToken = request.RefreshToken });

                if (user == null)
                    return BadRequest(new { request, Message = "Refresh Token is incorrect" });

                await _jwtUtil.RemoveRefreshTokenAsync(user.Id, request.RefreshToken);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return StatusCode(500);
            }
        }
    }
}
