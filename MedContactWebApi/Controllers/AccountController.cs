using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Commands;
using MedContactDataCQS.Users.Queries;
using MedContactDb.Entities;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models.Requests;
using MedContactWebApi.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly DataChecker _datachecker;
        private readonly IMediator _mediator;

        /// <summary>
        /// Account Controller Ctor
        /// </summary>
        public AccountController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, DataChecker datachecker)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
            _datachecker = datachecker;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="model CustomerRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CustomerRegRequestModel model)
        {
                try
                {
                    var customerDto = _mapper.Map<UserDto>(model);
                    if (customerDto == null )
                        return new BadRequestObjectResult(new TextResponse() { Message = "RegData is null" });
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                         return new BadRequestObjectResult(new TextResponse()
                         { Message = "Email or Password is null or empty" });
                 var resMail = await  _datachecker.CheckEmail(customerDto.Email, HttpContext);
                if (resMail == null)
                    return new BadRequestObjectResult(new TextResponse() { Message = "Email check error" });
                if (resMail.isMatched == false)
                    return new BadRequestObjectResult(resMail);
                var resAge = _datachecker.BirthDateCheck(customerDto.BirthDate, false);
                if (resAge == null)
                    return new BadRequestObjectResult(new TextResponse() { Message = "Birth date check error" });
                if (resAge.isMatched == false)
                    return new BadRequestObjectResult(resAge);

                customerDto.PasswordHash = _md5.CreateMd5(model.Password);
                   var res = await _mediator.Send(new CreateUserWithRoleCommand() 
                                   { UserDto = customerDto, RoleName = "Customer" });
                if (res > 0)
                {
                    var role = await _mediator.Send(new GetRoleByNameQuery() { RoleName = "Customer" });

                    if (role == null)
                        return BadRequest(new ErrorModel() { Message = "User's roles are not found" });
                    var response = await _jwtUtil.GenerateTokenAsync(customerDto, new List<RoleDto> { role });
                    return Ok(response);
                }
                else
                    return BadRequest(new ErrorModel() { Message = "User is not created" });
                    
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
        }


        /// <summary>
        /// UserLoginPreview
        /// </summary>
        /// <returns> UserDataModelResponse</returns>
        [HttpGet]
        public async Task<IActionResult> UserLoginPreview()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {

                var sUserId = User.FindFirst("UId");
                var sMainUserId = User.FindFirst("MUId");

                if (sUserId == null)
                {
                    return BadRequest();
                }

                if (sMainUserId == null)
                {
                    return BadRequest();
                }
                try
                {

                    if (sUserId != null && sMainUserId != null)
                    {
                        if (sUserId.Value.Equals(sMainUserId.Value))
                        {
                            if (Guid.TryParse(sUserId!.Value, out Guid userId))
                            {
                                var user = await _mediator.Send(new GetUserByIdQuery() { UserId= userId});
                                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                                if (user != null && roles != null)
                                {
                                    string fName = user.Name + " " + user.Surname;
                                    UserDataModelResponse model = new()
                                    {
                                        ActiveEmail = user.Email,
                                        ActiveFullName = fName,
                                        MainEmail = user.Email,
                                        MainFullName = fName,
                                        RoleNames = roles
                                    };
                                    return Ok(model);
                                }
                            }
                        }
                        else
                        {
                            UserDataModelResponse model = new ();

                            if (Guid.TryParse(sUserId!.Value, out Guid userId))
                            {
                                var user = await _mediator.Send(new GetUserByIdQuery() { UserId = userId });
                                if (user != null)
                                {
                                    string fName = user.Name + " " + user.Surname;
                                    model.ActiveFullName = fName;
                                    model.ActiveEmail = user.Email;
                                }
                            }
                            if (Guid.TryParse(sMainUserId!.Value, out Guid mainUserId))
                            {
                                var mainUser = await _mediator.Send(new GetUserByIdQuery() { UserId = mainUserId });
                                if (mainUser != null)
                                {
                                    string fName = mainUser.Name + " " + mainUser.Surname;
                                    model.MainFullName = fName;
                                    model.MainEmail = mainUser.Email;
                                }
                            }
                            return Ok(model);
                        }
                    }
                }

                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                }
            }

            return Ok();
        }
    }
}
