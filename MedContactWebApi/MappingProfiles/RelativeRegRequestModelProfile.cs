using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using MedContactWebApi.Models;

namespace MedContactWebApi.MappingProfiles
{

    /// <summary>
    /// RelativeRegRequestModel Mapping to UserDto
    /// </summary>
    public class RelativeRegRequestModelProfile : Profile
    {
        /// <summary>
        /// CustomerRegRequestModel Profile Mapping to UserDto
        /// </summary>
        public RelativeRegRequestModelProfile()
        {
            CreateMap<RelativeRegRequestModel, UserDto>()
                .ForMember(dto => dto.IsFullBlocked,
                     opt => opt.MapFrom(s => false))
                .ForMember(dto => dto.RegistrationDate,
                     opt => opt.MapFrom(model => DateTime.Now))
                .ForMember(dto => dto.Id,
                     opt => opt.MapFrom(model =>Guid.NewGuid()))
                .ForMember(dto => dto.IsDependent,
                     opt => opt.MapFrom(model => true));


          

        }

    }
}
