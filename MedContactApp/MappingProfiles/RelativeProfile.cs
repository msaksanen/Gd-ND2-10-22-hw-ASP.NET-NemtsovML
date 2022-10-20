using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class RelativeProfile : Profile

    {
        public RelativeProfile()
        {


            CreateMap<RelativeModel, UserDto>()
                   .ForMember(dto => dto.RegistrationDate,
                     opt => opt.MapFrom(model => model.RegistrationDate ?? DateTime.Now))
                   .ForMember(dto => dto.Id,
                     opt => opt.MapFrom(model => model.Id ?? Guid.NewGuid()));

            CreateMap<UserDto, RelativeModel>().ReverseMap();
        }
    }
}
