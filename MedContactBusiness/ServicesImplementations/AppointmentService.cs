using AutoMapper;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;
using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq.Expressions;
using System.Security.Principal;

namespace MedContactBusiness.ServicesImplementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }


        public async Task<List<AppointmentDto>?> GetAppointmentsByDTTableIdAsync(Guid dttId)
        {
            var list = await _unitOfWork.AppointmentRepository.Get()
                                        .Include(a => a.CustomerData)
                                        .ThenInclude(c => c!.User)
                                        .Where(a => a.DayTimeTableId != null && a.DayTimeTableId.Equals(dttId))
                                        .Select(a => _mapper.Map<AppointmentDto>(a))
                                        .ToListAsync();
            return list;
        }

        public async Task<List<AppointmentDto>?> GetPatientsByDoctorDataId(Guid dataId)
        {
            var flist = await  _unitOfWork.AppointmentRepository.Get()
                                              .Where(d => d.DayTimeTable != null &&
                                                     d.DayTimeTable.DoctorDataId != null &&
                                                     d.DayTimeTable.DoctorDataId.Equals(dataId))
                                              .Include(a => a.CustomerData)
                                              .ThenInclude(c => c!.User)
                                              .ToListAsync();

            List<AppointmentDto>? list = flist.Select(a => _mapper.Map<AppointmentDto>(a))
                                              .OrderBy(a => a.CustomerData!.UserId)
                                              .ThenByDescending(a=>a.StartTime)
                                              .ToList();
                                         
         
            return list;
        }

        public async Task<int> UpdateRange(IEnumerable<AppointmentDto> apms)
        {
            var list = apms.Select(a => _mapper.Map<Appointment>(a));
            _unitOfWork.AppointmentRepository.UpdateRange(list);
            var res = await _unitOfWork.Commit();

            return res;
        }

        public async Task<int> Add(AppointmentDto apm)
        {
            var ent = _mapper.Map<Appointment>(apm);

            await _unitOfWork.AppointmentRepository.AddAsync(ent);
            if (ent.DayTimeTableId != null)
            {
                var dtt = await _unitOfWork.DayTimeTableRepository.GetByIdTrackAsync((Guid)ent?.DayTimeTableId!);
                if (dtt != null) dtt.FreeTicketQty--;
            }

            var res = await _unitOfWork.Commit();

            return res;
        }
        public async Task<int?> RemoveById(Guid apmId)
        {
            var ent = await _unitOfWork.AppointmentRepository.GetByIdAsync(apmId);   

            if (ent != null && ent.DayTimeTableId!=null)
            {
                var dtt = await _unitOfWork.DayTimeTableRepository.GetByIdTrackAsync((Guid)ent.DayTimeTableId!);
                if (dtt != null) dtt.FreeTicketQty++;
                _unitOfWork.AppointmentRepository.Remove(ent);
            }

            var res = await _unitOfWork.Commit();

            return res;
        }

        public async Task<IEnumerable<AppointmentDto>?> GetAppointmentsByUserId(Guid usrId)
        {
            var customerData = await _unitOfWork.CustomerDataRepository.Get().FirstOrDefaultAsync(c => c.UserId.Equals(usrId)); 
            if (customerData == null)
                return null;
            var list =    _unitOfWork.AppointmentRepository.Get()
                             .Include(a => a.DayTimeTable)
                                .ThenInclude(d => d!.DoctorData)
                                     .ThenInclude(x=>x!.Speciality).
                              Include(a => a.DayTimeTable)
                                .ThenInclude(d => d!.DoctorData)
                                     .ThenInclude(x => x!.User)
                             .Where(a => a.CustomerDataId.Equals(customerData.Id))
                             .Select(a => _mapper.Map<AppointmentDto>(a))
                             .AsEnumerable();
            return list;

        }

        public async Task<IEnumerable<AppointmentInfo>?> GetAppointmentMailListAsync(int daysTerm)
        {
            var list = await  _unitOfWork.AppointmentRepository.Get()
                                  .Where(a => a.StartTime!=null && a.StartTime.Value.Date==DateTime.Now.Date.AddDays(daysTerm))
                                  .Include(a => a.CustomerData)
                                    .ThenInclude(c => c!.User)
                                  .Include(a => a.DayTimeTable)
                                    .ThenInclude(d => d!.DoctorData)
                                      .ThenInclude(x => x!.User)
                                  .Include(a => a.DayTimeTable)
                                    .ThenInclude(d => d!.DoctorData)
                                        .ThenInclude(d => d!.Speciality)
                                  .ToListAsync();
            var apmInfo = list?.Select(x => _mapper.Map<AppointmentInfo>
                                              ((x, x?.DayTimeTable?.DoctorData, x?.CustomerData)))
                                              .ToList();

            return apmInfo;
        }
    }
}
