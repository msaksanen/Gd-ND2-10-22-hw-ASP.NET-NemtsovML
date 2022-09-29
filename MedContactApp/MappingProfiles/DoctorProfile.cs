using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {

            CreateMap<Doctor, DoctorDto>();

            CreateMap<DoctorDto, Doctor>().ReverseMap();

            CreateMap<DoctorModel, DoctorDto>()
                 .ForMember(dto => dto.Role,
                  opt => opt.MapFrom(model => "Doctor"))
                 .ForMember(dto => dto.RegistrationDate,
                  opt => opt.MapFrom(model => DateTime.Now))
                 .ForMember(dto => dto.Id,
                  opt => opt.MapFrom(model => Guid.NewGuid()))
                 .ForMember(dto => dto.PasswordHash,
                  opt => opt.MapFrom(model => model.Password));
        }

    }
}
