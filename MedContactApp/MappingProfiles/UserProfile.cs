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
            CreateMap<UserDto, ApplicantModel>();

            CreateMap<BaseUserModel, ChangePasswordModel>();
            CreateMap<BaseUserModel, RegDoctorDataModel>();
            CreateMap<BaseUserModel, AdminUserEditModel>();

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

            CreateMap<UserDto, UserMedDataModel>()
              .ForMember(m => m.UserId,
               opt => opt.MapFrom(s => s.Id))
              .ForMember(m => m.CustomerDataId,
               opt => opt.MapFrom(s => s.CustomerDataId))
              .ForMember(m => m.Name,
               opt => opt.MapFrom(s => s.Name))
              .ForMember(m => m.Surname,
               opt => opt.MapFrom(s => s.Surname))
              .ForMember(m => m.MidName,
               opt => opt.MapFrom(s => s.MidName))
             .ForMember(m => m.Email,
               opt => opt.MapFrom(s => s.Email));

            CreateMap<(UserDto, MedDataDto, List<FileDataDto>), UserMedDataModel>()
             .ForMember(m => m.UserId,
              opt => opt.MapFrom(s => s.Item1.Id))
             .ForMember(m => m.MedDataId,
              opt => opt.MapFrom(s => s.Item2.Id))
             .ForMember(m => m.CustomerDataId,
              opt => opt.MapFrom(s => s.Item1.CustomerDataId))
             .ForMember(m => m.Name,
              opt => opt.MapFrom(s => s.Item1.Name))
             .ForMember(m => m.Surname,
              opt => opt.MapFrom(s => s.Item1.Surname))
             .ForMember(m => m.MidName,
              opt => opt.MapFrom(s => s.Item1.MidName))
             .ForMember(m => m.Email,
              opt => opt.MapFrom(s => s.Item1.Email))
             .ForMember(m => m.fileDatas,
              opt => opt.MapFrom(s => s.Item3))
             .ForMember(m => m.Department,
              opt => opt.MapFrom(s => s.Item2.Department))
             .ForMember(m => m.InputDate,
              opt => opt.MapFrom(s => s.Item2.InputDate))
             .ForMember(m => m.ShortSummary,
              opt => opt.MapFrom(s => s.Item2.ShortSummary))
             .ForMember(m => m.TextData,
              opt => opt.MapFrom(s => s.Item2.TextData))
             .ForMember(m => m.Type,
              opt => opt.MapFrom(s => s.Item2.Type));

        }
    }
}
