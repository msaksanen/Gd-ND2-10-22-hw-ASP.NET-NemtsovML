using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class SpecialityProfile : Profile
    {
        public SpecialityProfile()
        {
            CreateMap<Speciality, SpecialityDto>();

            CreateMap<SpecialityDto, Speciality>().ReverseMap();

            CreateMap<UserDto, AdminEditDoctorModel>()
                .ForMember(m => m.UserId,
                 opt => opt.MapFrom(s => s.Id))
                .ForMember(m => m.Email,
                 opt => opt.MapFrom(s => s.Email))
                .ForMember(m => m.Name,
                 opt => opt.MapFrom(s => s.Name))
                .ForMember(m => m.Surname,
                 opt => opt.MapFrom(s => s.Surname))
                .ForMember(m => m.MidName,
                 opt => opt.MapFrom(s => s.MidName))
                .ForMember(m => m.BirthDate,
                 opt => opt.MapFrom(s => s.BirthDate))
                .ForMember(m => m.IsFullBlocked,
                 opt => opt.MapFrom(s => s.IsFullBlocked))
                .ForMember(m => m.Gender,
                 opt => opt.MapFrom(s => s.Gender));


        }
   
    }
}
