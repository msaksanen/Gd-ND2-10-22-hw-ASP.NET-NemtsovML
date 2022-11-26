using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using MedContactWebApi.Models;

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

            CreateMap<BaseUserModel, UserDto>()
                  .ForMember(dto => dto.RegistrationDate,
                    opt => opt.MapFrom(model => model.RegistrationDate ?? DateTime.Now))
                  .ForMember(dto => dto.Id,
                    opt => opt.MapFrom(model => model.Id ?? Guid.NewGuid()));
                  //.ForMember(dto => dto.PasswordHash,
                  //  opt => opt.MapFrom(model => model.Password));

            CreateMap<UserDto, BaseUserModel>().ReverseMap();

            CreateMap<UserDto, ChangePasswordModel>();

        }

    }
}
