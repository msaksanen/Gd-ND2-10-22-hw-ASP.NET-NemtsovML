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
using MedContactWebApi.Models.Requests;
using MedContactWebApi.Models.Responses;
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
                return new BadRequestObjectResult(new TextResponse() { Message = "RegData is null" });
            if (string.IsNullOrEmpty(model.Email))
                return new BadRequestObjectResult(new TextResponse()
                { Message = "Email is null or empty" });

            try
            {                 
                    var relativeDto = _mapper.Map<UserDto>(model);
                    if (relativeDto != null)
                    {
                     var resMail = await _datachecker.CheckEmail(relativeDto.Email, HttpContext);
                     if (resMail == null)
                        return new BadRequestObjectResult(new TextResponse() { Message = "Email check error" });
                     if (resMail.isMatched == false)
                        return new BadRequestObjectResult(resMail);
                     var resAge = _datachecker.BirthDateCheck(relativeDto.BirthDate, true);
                     if (resAge == null)
                        return new BadRequestObjectResult(new TextResponse() { Message = "Birth date check error" });
                     if (resAge.isMatched == false)
                        return new BadRequestObjectResult(resAge);

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
                                return Ok(new TextResponse() { Message = "Relative was added to existing family" });
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
                                return Ok(new TextResponse() { Message = "Relative was added to new family" });
                            }
                        }
                        
                    }
            }
            catch (Exception e)
            {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
            }
      
            return  BadRequest(new ErrorModel() {Message = "Something went wrong" });
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

    }
}
