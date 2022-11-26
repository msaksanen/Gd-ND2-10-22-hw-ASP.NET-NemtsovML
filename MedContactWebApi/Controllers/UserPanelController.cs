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
using MedContactWebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// UserPanel Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserPanelController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly DataChecker _datachecker;
        private readonly IMediator _mediator;
        private readonly ModelUserBuilder _modelBuilder;

        /// <summary>
        /// Account Controller Ctor
        /// </summary>
        public UserPanelController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, DataChecker datachecker,
                                   ModelUserBuilder modelBuilder)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
            _datachecker = datachecker;
            _modelBuilder = modelBuilder;
        }



        /// <summary>
        /// AccSettings 
        /// </summary>
        /// <param name="id string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AccSettings([FromQuery] string? id)
        {
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model != null)
                return Ok(model);

            return NotFound(new ErrorModel() { Message = "User is not found" });
        }


        /// <summary>
        /// AccSettingsEdit
        /// </summary>
        /// <param name="id string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AccSettingsEdit([FromQuery] string? id)
        {
            var model = await _modelBuilder.BuildById(HttpContext, id);
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound("User is not found");
        }


        /// <summary>
        /// AccSettingsEdit
        /// </summary>
        /// <param name="model BaseUserModel"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> AccSettingsEdit([FromBody] BaseUserModel model)
        {
            try
            {
                if (model != null)
                {
                    var dto = _mapper.Map<UserDto>(model);
                    var sourceDto = await _mediator.Send(new GetUserByIdQuery() { UserId = dto.Id });
                    if (sourceDto == null)
                         return NotFound (new {model, Message = "User (source) is not found" });

                    if (string.IsNullOrEmpty(model.Email))
                        return  BadRequest (new { model, Message = "Email is null or empty" });

                    var resMail = await _datachecker.CheckEmail(dto.Email, HttpContext);
                    if (resMail == null)
                        return BadRequest(new {model,  Message = "Email check error" });
                    if (resMail.isMatched == false)
                        return BadRequest(new {model, resMail });
                    var resAge = _datachecker.BirthDateCheck(dto.BirthDate, dto.IsDependent);
                    if (resAge == null)
                        return BadRequest(new {model,  Message = "Birth date check error" });
                    if (resAge.isMatched == false)
                        return BadRequest(new {model, resAge });

                    dto.RegistrationDate = sourceDto.RegistrationDate;

                    PatchMaker<UserDto> patchMaker = new();
                    var patchList = patchMaker.Make(dto, sourceDto);
                    var resPatch = await _mediator.Send(new PatchUserDataCommand() { UserId = dto.Id, PatchList = patchList });  
                    if (resPatch == null || resPatch==0)
                        return BadRequest(new {model,  Message = "User data changes were not saved" });

                    return Ok(new {model, Message = "User data changes were saved" , MatchResult =resPatch });
                }
                else
                {
                    return BadRequest(new {model, Message = "Model is null" });
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }

        /// <summary>
        /// ChangePassword Get
        /// </summary>
        /// <param name="id UserId string?"></param>
        /// <returns>ChangePasswordModel</returns>
        [HttpGet]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> ChangePassword(string? id)
        {
            var model = await _modelBuilder.BuildUserDtoById(HttpContext, id);
            if (model != null)
            {
                var chModel = _mapper.Map<ChangePasswordModel>(model);
                return Ok(chModel);
            }

            return NotFound(new ErrorModel {Message="User is not found"});
        }

        /// <summary>
        /// ChangePassword Post
        /// </summary>
        /// <param name="model ChangePasswordModel"></param>
        /// <returns>ChangePasswordModel</returns>
        [HttpPost]
        [Authorize(Policy = "FullBlocked")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (model != null && model.Id != null && model.Password != null && model.OldPassword != null)
            {
                if (!model.Password.Equals(model.PasswordConfirmation))
                {
                    model.SystemInfo = "You new password and confirmation differs";
                    return BadRequest(model);
                }

                try
                {
                    var pwdHash = _md5.CreateMd5(model.OldPassword);
                    var res = await _mediator.Send(new CheckUserPwdHashByUserIdQuery() { passwordHash = pwdHash, UserId = model.Id });
                    if (res==true)
                    {
                        var newPwdHash = _md5.CreateMd5(model.Password);
                        var result = await _mediator.Send(new ChangeUserPasswordCommand() { UserId = model.Id, passwordHash = newPwdHash });
                        if (result > 0)
                        {
                            model.SystemInfo = "The password has been changed successfully";
                            return Ok(model);
                        }
                    }
                    else
                    {
                        model.SystemInfo = "You have entered incorrect password";
                        return BadRequest(model);
                    }

                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }

            ChangePasswordModel model1 = new() { SystemInfo = "Something went wrong (." };
            return BadRequest(model1);
        }


    }
}
