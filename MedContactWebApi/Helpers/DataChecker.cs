using AutoMapper;
using MedContactCore.Abstractions;
using MedContactDataCQS.Family.Queries;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Queries;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models.Requests;
using MedContactWebApi.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// DataChecker Helper
    /// </summary>
    public class DataChecker 
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly IMediator _mediator;

        /// <summary>
        /// DataChecker  Ctor
        /// </summary>
        public DataChecker(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
        }


        /// <summary>
        /// CheckEmail by RegEx and Database
        /// </summary>
        /// <param name="email string?"></param>
        /// <param name="context HttpContext"></param>
        /// <returns>TextResponse(Message, isMatched)</returns>
        public async Task<TextResponse> CheckEmail(string? email, HttpContext context)
        {
                if (string.IsNullOrEmpty(email))
                    return new TextResponse() { Message = "Email is null or empty", isMatched = false };
                if (!IsValidEmail(email))
                    return new TextResponse() { Message = "Email is incorrect", isMatched = false };

                var curEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
                if (curEmail != null && curEmail.Equals(email))
                    return new TextResponse() { isMatched = true };

                var mainUserId = context.User.FindFirst("MUId")?.Value;

                if (Guid.TryParse(mainUserId, out Guid Uid))
                {
                    var family = await _mediator.Send(new GetRelativesListQuery() { MainUserId = Uid });
                    if (family != null && family.Any(r => r.Email != null && r.Email.Equals(email)))
                        return new TextResponse() { isMatched = true };
                }

                var res = await _mediator.Send(new CheckUserEmailQuery() { Email = email });
                if (res == true)
                    return new TextResponse() {Message = "Email is already in use", isMatched = false };

                return new TextResponse() { isMatched = true };
        }

        /// <summary>
        /// CheckEmail by RegEx and Database
        /// </summary>
        /// <param name="birthDate DateTime? "></param>
        /// <param name="isDependent bool?"></param>
        ///  MatchResult = 0:  - incorrect or null
        ///  MatchResult = 1:  - OK
        ///  MatchResult = 2 : age less or equal 17
        /// <returns>TextResponse(Message, isMatched,MatchResult)</returns>
        public TextResponse BirthDateCheck(DateTime? birthDate, bool? isDependent)
        {

                if (birthDate == null || isDependent == null)
                    return new TextResponse()
                    { Message = "BirthDate or isDependent are null", isMatched = false, MatchResult = 0 };

                if (isDependent == true)
                    return new TextResponse() { isMatched = true, MatchResult = 0 };

                var diff = (DateTime.Now).Subtract((DateTime)birthDate);
                var age = diff.TotalDays / 365;
                if (age <= 17)
                    return new TextResponse()
                    { Message = "Registration is for adults only", isMatched = false, MatchResult = 2 };

                if (birthDate <= DateTime.Now && age <= 120)
                    return new TextResponse() { isMatched = true, MatchResult = 1 };

                return new TextResponse()
                { Message = "Input correct date of birth", isMatched = false, MatchResult = 0 };
           

        }

        internal static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return false;
            }
            catch (ArgumentException e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException e )
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return false;
            }
        }
    }


   
}
