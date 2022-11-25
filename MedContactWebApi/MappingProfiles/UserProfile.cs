using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.MappingProfiles
{

    /// <summary>
    /// User Mapping to UserDto
    /// </summary>
    public class UserProfile : Profile
    {
        /// <summary>
        /// User Profile Mapping to UserDto
        /// </summary>
        public UserProfile()
        {
            CreateMap<User, UserDto>()
               .ForMember(dto => dto.CustomerDataId,
                    opt => opt.MapFrom(e => e.CustomerData!.Id));

            CreateMap<UserDto, User>().ReverseMap();

        }

    }
}
