using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// AppointmentProfile
    /// </summary>
    public class AppointmentProfile : Profile
    {
        /// <summary>
        /// AppointmentProfile Ctor
        /// </summary>
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentDto>();

            CreateMap<AppointmentDto, Appointment>().ReverseMap();

            CreateMap<(AppointmentDto, DoctorDataDto, CustomerDataDto), AppointmentInfo>()
               .ForMember(i => i.Id, opt => opt.MapFrom(d => d.Item1.Id))
               .ForMember(i => i.CreationDate, opt => opt.MapFrom(d => d.Item1.CreationDate))
               .ForMember(i => i.StartTime, opt => opt.MapFrom(d => d.Item1.StartTime))
               .ForMember(i => i.EndTime, opt => opt.MapFrom(d => d.Item1.EndTime))
               .ForMember(i => i.DayTimeTableId, opt => opt.MapFrom(d => d.Item1.DayTimeTableId))
               .ForMember(i => i.DoctorDataId, d => d.MapFrom(dt => dt.Item2.Id))
               .ForMember(i => i.DoctorSpeciality, d => d.MapFrom(dt => (dt.Item2.Speciality != null) ? (dt.Item2.Speciality.Name ?? String.Empty) : String.Empty))
               .ForMember(i => i.DoctorName, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Name ?? String.Empty) : String.Empty))
               .ForMember(i => i.DoctorMidname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.MidName ?? String.Empty) : String.Empty))
               .ForMember(i => i.DoctorSurname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Surname ?? String.Empty) : String.Empty))
               .ForMember(i => i.CustomerDataId, d => d.MapFrom(dt => dt.Item1.CustomerDataId))
               .ForMember(i => i.CustomerName, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Name ?? String.Empty) : String.Empty))
               .ForMember(i => i.CustomerMidname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.MidName ?? String.Empty) : String.Empty))
               .ForMember(i => i.CustomerSurname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Surname ?? String.Empty) : String.Empty))
               .ForMember(i => i.CustomerBirthDate, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.BirthDate ?? DateTime.MinValue) : DateTime.MinValue));
        }
   
    }
}
