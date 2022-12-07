using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class MedDataProfile : Profile
    {
        public MedDataProfile()
        {
            CreateMap<MedData, MedDataDto>();

            CreateMap<MedDataDto, MedData>().ReverseMap();
            CreateMap<(MedDataDto, DoctorDataDto, CustomerDataDto, UserDto), MedDataInfo>()
             .ForMember(i => i.Id, opt => opt.MapFrom(d => d.Item1.Id))
             .ForMember(i => i.InputDate, opt => opt.MapFrom(d => d.Item1.InputDate))
             .ForMember(i => i.Department, opt => opt.MapFrom(d => d.Item1.Department))
             .ForMember(i => i.Type, opt => opt.MapFrom(d => d.Item1.Type))
             .ForMember(i => i.ShortSummary, opt => opt.MapFrom(d => d.Item1.ShortSummary))
             .ForMember(i => i.TextData, opt => opt.MapFrom(d => d.Item1.TextData))
             .ForMember(i => i.FileDatas, opt => opt.MapFrom(d => d.Item1.FileDatas))
             .ForMember(i => i.DoctorDataId, d => d.MapFrom(dt => dt.Item2.Id))
             .ForMember(i => i.DoctorSpeciality, d => d.MapFrom(dt => (dt.Item2.Speciality != null) ? (dt.Item2.Speciality.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorName, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorMidname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.MidName ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorSurname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Surname ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerDataId, d => d.MapFrom(dt => dt.Item1.CustomerDataId))
             .ForMember(i => i.CustomerName, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerMidname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.MidName ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerSurname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Surname ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerBirthDate, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.BirthDate ?? DateTime.MinValue) : DateTime.MinValue))
             .ForMember(i => i.UserId, d => d.MapFrom(dt => dt.Item4.Id));

            CreateMap<(MedData, DoctorData, CustomerData, User), MedDataInfo>()
             .ForMember(i => i.Id, opt => opt.MapFrom(d => d.Item1.Id))
             .ForMember(i => i.InputDate, opt => opt.MapFrom(d => d.Item1.InputDate))
             .ForMember(i => i.Department, opt => opt.MapFrom(d => d.Item1.Department))
             .ForMember(i => i.Type, opt => opt.MapFrom(d => d.Item1.Type))
             .ForMember(i => i.ShortSummary, opt => opt.MapFrom(d => d.Item1.ShortSummary))
             .ForMember(i => i.TextData, opt => opt.MapFrom(d => d.Item1.TextData))
             .ForMember(i => i.FileDatas, opt => opt.MapFrom(d => d.Item1.FileDatas))
             .ForMember(i => i.DoctorDataId, d => d.MapFrom(dt => dt.Item2.Id))
             .ForMember(i => i.DoctorSpeciality, d => d.MapFrom(dt => (dt.Item2.Speciality != null) ? (dt.Item2.Speciality.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorName, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorMidname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.MidName ?? String.Empty) : String.Empty))
             .ForMember(i => i.DoctorSurname, d => d.MapFrom(dt => (dt.Item2.User != null) ? (dt.Item2.User.Surname ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerDataId, d => d.MapFrom(dt => dt.Item1.CustomerDataId))
             .ForMember(i => i.CustomerName, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Name ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerMidname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.MidName ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerSurname, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.Surname ?? String.Empty) : String.Empty))
             .ForMember(i => i.CustomerBirthDate, d => d.MapFrom(dt => (dt.Item3.User != null) ? (dt.Item3.User.BirthDate ?? DateTime.MinValue) : DateTime.MinValue))
             .ForMember(i => i.UserId, d => d.MapFrom(dt => dt.Item4.Id));

        }

    }
}
