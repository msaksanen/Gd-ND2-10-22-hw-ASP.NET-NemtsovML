using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class UserProfile : Profile

    {
        public UserProfile()
        {

            CreateMap<User, UserDto>()
                .ForMember(dto => dto.CustomerDataId,
                     opt => opt.MapFrom(e => e.CustomerData!.Id));

            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<BaseUserModel, UserDto>()
                   .Include<UserModel, UserDto>()
                   .Include<CustomerModel, UserDto>()
                   .Include<DoctorModel, UserDto>()
                   .ForMember(dto => dto.RegistrationDate,
                     opt => opt.MapFrom(model => model.RegistrationDate ?? DateTime.Now))
                   .ForMember(dto => dto.Id,
                     opt => opt.MapFrom(model => model.Id ?? Guid.NewGuid()))
                   .ForMember(dto => dto.PasswordHash,
                     opt => opt.MapFrom(model => model.Password));

            CreateMap<UserDto, BaseUserModel>().ReverseMap();

            CreateMap<UserModel, UserDto>();

            CreateMap<CustomerModel, UserDto>();
            CreateMap<UserDto,CustomerModel>().ReverseMap();

            CreateMap<DoctorModel, UserDto>();
            
            CreateMap<UserDto, UserDataModel>();
            CreateMap<UserDto, AdminUserEditModel>();

            CreateMap<BaseUserModel, ChangePasswordModel>();
            CreateMap<BaseUserModel, RegDoctorDataModel>();
            CreateMap<BaseUserModel, AdminUserEditModel>();

        }
    }
}
