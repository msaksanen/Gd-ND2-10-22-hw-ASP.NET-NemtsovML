using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class DoctorFullDataProfile : Profile
    {
        public DoctorFullDataProfile()
        {
            CreateMap<(DoctorData, User, Speciality), DoctorFullDataDto>()
                .ForMember(dto => dto.Id,
                 opt => opt.MapFrom(d => d.Item1.Id))
                .ForMember(dto => dto.UserId,
                 opt => opt.MapFrom(d => d.Item2.Id))
                .ForMember(dto => dto.Username,
                 opt => opt.MapFrom(d => d.Item2.Name))
                .ForMember(dto => dto.Surname,
                 opt => opt.MapFrom(d => d.Item2.Surname))
                .ForMember(dto => dto.MidName,
                 opt => opt.MapFrom(d => d.Item2.MidName))
                 .ForMember(dto => dto.Email,
                 opt => opt.MapFrom(d => d.Item2.Email))
                .ForMember(dto => dto.BirthDate,
                 opt => opt.MapFrom(d => d.Item2.BirthDate))
                .ForMember(dto => dto.Gender,
                 opt => opt.MapFrom(d => d.Item2.Gender))
                .ForMember(dto => dto.Roles,
                 opt => opt.MapFrom(d => d.Item2.Roles))
                .ForMember(dto => dto.IsFullBlocked,
                 opt => opt.MapFrom(d => d.Item2.IsFullBlocked))
                .ForMember(dto => dto.SpecialityName,
                 opt => opt.MapFrom(d => d.Item3.Name))
                .ForMember(dto => dto.IsBlocked,
                 opt => opt.MapFrom(d => d.Item1.IsBlocked))
                 .ForMember(dto => dto.DayTimeTables,
                 opt => opt.MapFrom(d => d.Item1.DayTimeTables))
                   .ForMember(dto => dto.SpecialityId,
                 opt => opt.MapFrom(d => d.Item1.SpecialityId));

            CreateMap<User, DoctorFullDataDto>()
                  .ForMember(dto => dto.UserId,
                 opt => opt.MapFrom(d => d.Id))
                .ForMember(dto => dto.Username,
                 opt => opt.MapFrom(d => d.Name))
                  .ForMember(dto => dto.SpecialityName,
                 opt => opt.MapFrom(d=>"Underfined"));

        }
   
    }
}
