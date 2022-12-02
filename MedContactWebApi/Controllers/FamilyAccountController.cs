using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.Family.Commands;
using MedContactDataCQS.Family.Queries;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Commands;
using MedContactDataCQS.Users.Queries;
using MedContactDb.Entities;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// FamilyAccountController 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FamilyAccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly IMediator _mediator;
        private readonly DataChecker _datachecker;

        /// <summary>
        /// FamilyAccount Controller Ctor
        /// </summary>
        public FamilyAccountController (IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, DataChecker dataChecker)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;
            _datachecker = dataChecker;
        }



        /// <summary>
        /// Family - List of relatives
        /// </summary>
        /// <returns> Collection of relatives</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Family()
        {
            var mainUserId = User.FindFirst("MUId");
            try
            {
               var res = Guid.TryParse(mainUserId!.Value, out Guid MUid);
                if (!res)
                    return new BadRequestObjectResult( new ErrorModel { Message = "Main User is not found"});

                var customers = await _mediator.Send(new GetRelativesListQuery() { MainUserId =MUid });
                return Ok(customers);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        /// <summary>
        /// AddRelative to family
        /// </summary>
        /// <param name="model RelativeRegRequestModel"></param>
        /// <returns> IActionResult </returns>
        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AddRelative([FromBody] RelativeRegRequestModel model)
        {
            if (model == null)
                return BadRequest(new { model, Message = "RegData is null" });
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new {model, Message = "Email is null or empty" });

            try
            {                 
                    var relativeDto = _mapper.Map<UserDto>(model);
                    if (relativeDto != null)
                    {
                     var resMail = await _datachecker.CheckEmail(relativeDto.Email, HttpContext);
                     if (resMail == null)
                        return BadRequest(new {model, Message = "Email check error" });
                     if (resMail.isMatched == false)
                        return BadRequest(new { model, resMail });
                     var resAge = _datachecker.BirthDateCheck(relativeDto.BirthDate, true);
                     if (resAge == null)
                        return BadRequest(new {model, Message = "Birth date check error" });
                     if (resAge.isMatched == false)
                        return BadRequest(new { model, resAge });

                     int? result=0;
                        var mainUserId = User.FindFirst("MUId");
                        Guid MUid = Guid.Parse(mainUserId!.Value);
                        var mainUserDto = await _mediator.Send(new GetUserByIdQuery() {UserId = MUid });
                        if (mainUserDto != null)
                        {
                            if (mainUserDto.FamilyId != null)
                            {
                                relativeDto.FamilyId = mainUserDto.FamilyId;
                                result = await _mediator.Send(new CreateUserWithRoleCommand() { UserDto = relativeDto, RoleName = "Customer" });
                            if (result > 0)
                                return Ok(new{model, Message = "Relative was added to existing family" });
                            }
                            else
                            {
                                FamilyDto newfamilyDto = new FamilyDto() { Id = Guid.NewGuid(), MainUserId = mainUserDto.Id };
                                mainUserDto.FamilyId = newfamilyDto.Id;
                                relativeDto.FamilyId = newfamilyDto.Id;
                                result = await _mediator.Send(new CreateNewFamilyForMainUserCommand() 
                                                {MainUserDto = mainUserDto, FamilyDto = newfamilyDto });
                                result += await _mediator.Send(new CreateUserWithRoleCommand() 
                                                { UserDto = relativeDto, RoleName = "Customer" });
                                if (result > 3)
                                return Ok(new { model, Message = "Relative was added to new family" });
                            }
                        }
                        
                    }
            }
            catch (Exception e)
            {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
            }
      
            return  BadRequest(new {model, Message = "Something went wrong" });
        }



        /// <summary>
        /// SetActiveRelative by Id or by Claims
        /// </summary>
        /// <param name="id [FromQuery]"></param>
        /// <returns> IActionResult </returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> SetActiveRelative([FromQuery] string? id)
        {
            var mainUserId = User.FindFirst("MUId");
            UserDto? dto = new();
            var res = Guid.TryParse(mainUserId!.Value, out Guid MUid);
            if (!res)
                return BadRequest(new ErrorModel() { Message = "Main user Id is incorrect" });

            var result = Guid.TryParse(id, out Guid relativeId);
   
            if (result)
            {
               dto = await _mediator.Send(new GetUserByIdQuery() { UserId = relativeId });
               if (dto == null)
                    return NotFound(new ErrorModel() { Message = "User is not found" });   
            }
            else
            {
                dto = await _mediator.Send(new GetUserByIdQuery() { UserId = MUid });
                if (dto == null)
                    return NotFound(new ErrorModel() { Message = "Main user is not found" });
            }


            var roleList = await _mediator.Send(new GetRoleListByUserIdQuery() { UserId = dto.Id });
            if (roleList == null || roleList.Count() == 0)
                return BadRequest(new ErrorModel() { Message = "User's roles are not found" });
            var response = await _jwtUtil.GenerateTokenAsync(dto, roleList, mainUserId.Value);
            return Ok(response);

        }

        /// <summary>
        /// RemoveAccount
        /// </summary>
        /// <param name="model RemoveAccountRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> RemoveAccount(RemoveAccountRequest model)
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated) &&
                 model != null && model.Id != null && model.Pwd != null && model.Keyword != null)
            {
                try
                {
                    var sMainUserId = User.FindFirst("MUId");
                    if (sMainUserId == null)
                    {
                        return BadRequest ("User is not found. Something went wrong");
                    }
                    var mRes = Guid.TryParse(sMainUserId.Value, out Guid MUId);
                    if (!mRes)
                    {
                        return BadRequest("User is not found. Something went wrong");
                    }
                    var pwdHash = _md5.CreateMd5(model.Pwd);
                    var res = await _mediator.Send(new CheckUserByUsernamePasswordQuery() 
                                     { Id=MUId, PwdHash = pwdHash,  Username = model.Keyword });
                    if (res == 0)
                    {
                        model.SystemInfo = "User is not found. Something went wrong";
                        return BadRequest(model);
                    }
                    if (res == 1)
                    {
                        model.SystemInfo = "You have entered wrong password or code name";
                        return BadRequest(model);
                    }
                    else
                    {
                        var relatives = await _mediator.Send(new GetRelativesListQuery() { MainUserId = model.Id });
                        if (relatives != null && relatives.Count > 1)
                        {
                            model.SystemInfo = "You should delete all accounts of your relatives at first";
                            return BadRequest(model);
                        }
                        else
                        {
                            var token = await _jwtUtil.GetRefreshToken(MUId);
                            var result = await _mediator.Send(new RemoveUserByIdCommand() { Id = model.Id });
                            if (result > 0)
                            {
                                if (MUId.Equals(model.Id))
                                {
                                    await _jwtUtil.RemoveRefreshTokenAsync(MUId, token);
                                    return Ok("Your account has been deleted");
                                }
                                else
                                {
                                    var usr = await _mediator.Send(new GetUserByIdQuery() { UserId = MUId });
                                    if (usr== null)
                                        return BadRequest("User is not found. Something went wrong");
                                    var roleList = await _mediator.Send(new GetRoleListByUserIdQuery() { UserId = usr.Id });
                                    if (roleList == null || roleList.Count() == 0)
                                        return BadRequest(new ErrorModel() { Message = "User's roles are not found" });
                                    var response = await _jwtUtil.GenerateTokenAsync(usr, roleList);
                                    return Ok(new { response, Message = "Account (relative) has been deleted" });
                                    }
                            }
                            else
                            {
                                model.SystemInfo = "Your account has not been removed<br/>Something went wrong";
                                return BadRequest(model);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }
            RemoveAccountRequest model1 = new() { SystemInfo = "Something went wrong" };
            return BadRequest(model1);
        }

    }
}
