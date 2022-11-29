using AutoMapper;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataCQS.DoctorData.Queries;
using MedContactDataCQS.Roles.Queries;
using MedContactDataCQS.Tokens.Commands;
using MedContactDataCQS.Tokens.Queries;
using MedContactDataCQS.Users.Commands;
using MedContactDataCQS.Users.Queries;
using MedContactDb.Entities;
using MedContactWebApi.AdminPanelHelpers;
using MedContactWebApi.FilterSortPageHelpers;
using MedContactWebApi.Helpers;
using MedContactWebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Serilog;
using System.Security.Claims;

namespace MedContactWebApi.Controllers
{
    /// <summary>
    /// Doctor Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly DataChecker _datachecker;
        private readonly IMediator _mediator;
        private readonly ModelUserBuilder _modelBuilder;
        private readonly AdminSortFilter _adminSortFilter;
        private readonly IConfiguration _configuration;
        private int _pageSize = 7;

        /// <summary>
        /// Doctor Controller Ctor
        /// </summary>
        public DoctorController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, 
            DataChecker datachecker, IConfiguration configuration, ModelUserBuilder modelBuilder,
            AdminSortFilter adminSortFilter)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5; 
            _mediator = mediator;   
            _datachecker = datachecker;
            _modelBuilder = modelBuilder;
            _configuration = configuration;
            _adminSortFilter = adminSortFilter;
        }


        /// <summary>
        /// Doctor Index
        /// </summary>
        /// <param name="model DoctorIndexRequestModel [FromQuery]"></param>
        /// string name, string surname, string speciality, int page = 1, SortState sortOrder = SortState.SpecialityAsc
        /// <returns></returns>
            [HttpGet]
        public async Task<IActionResult> Index([FromQuery]DoctorIndexRequestModel model)

        {
            try
            {
                bool result = int.TryParse(_configuration["PageSize:Default"], out var pageSize);
                if (result) _pageSize = pageSize;
                var dDatas = await _mediator.Send(new GetDoctorDataQuery());
                if (dDatas==null || !dDatas.Any())
                    return NotFound ("Doctor data is not found");
                                             
                dDatas = dDatas?.Where(d => d.SpecialityId != null && d.IsBlocked != true && d.ForDeletion != true);
                dDatas = _adminSortFilter.DoctorDataFilter(dDatas!, "", model.Name, model.Surname, model.Speciality);
                dDatas = _adminSortFilter.DoctorDataSort(dDatas, model.SortOrder);

                var count = await dDatas.CountAsync();
                var doctorDatas = await dDatas.Skip((model.Page - 1) * pageSize).Take(pageSize).ToListAsync();
                var items = doctorDatas.Select(dd => _mapper.Map<DoctorFullDataDto>((dd, dd.User, dd.Speciality)));

                string pageRoute = @"/doctor/index?page=";
                string processOptions = $"&name={model.Name}&speciality={model.Speciality}&sortorder={model.SortOrder}";

                string link = Request.Path.Value + Request.QueryString.Value;
                link = link.Replace("&", "*");
                //ViewData["Reflink"] = link;

                DoctDataIndexViewModel viewModel = new(
                    items, processOptions, link,
                    new PageViewModel(count, model.Page, pageSize, pageRoute),
                    new FilterSpecViewModel(null, null, model.Name, "", model.Speciality),
                    new SortViewModel(model.SortOrder)
                );
                return Ok(viewModel);

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

    }
}
