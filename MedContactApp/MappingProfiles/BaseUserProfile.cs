//using AutoMapper;
//using MedContactApp.Models;
//using MedContactCore.DataTransferObjects;
//using MedContactDb.Entities;

//namespace MedContactApp.MappingProfiles
//{
//    public class BaseUserProfile : Profile

//    {
//        public BaseUserProfile()
//        {

//            CreateMap<BaseUser, BaseUserDto>()
//            .Include<User, UserDto>()
//            .Include<Customer, CustomerDto>()
//            .Include<Doctor, DoctorDto>();

//            CreateMap<User, UserDto>();
//            CreateMap<Customer, CustomerDto>();
//            CreateMap<Doctor, DoctorDto>();
            

//            CreateMap<BaseUserDto, BaseUser>()
//            .Include<UserDto, User>()
//            .Include<CustomerDto, Customer>()
//            .Include<DoctorDto, Doctor>()
//            .ReverseMap();

//            CreateMap<UserDto, User>().ReverseMap();
//            CreateMap<CustomerDto, Customer>().ReverseMap();
//            CreateMap<DoctorDto, Doctor>().ReverseMap();

//            CreateMap<BaseUserModel, BaseUserDto>()
//                .Include<UserModel, UserDto>()
//                .Include<CustomerModel, CustomerDto>()
//                .Include<DoctorModel, DoctorDto>()
//                .ForMember(dto => dto.RegistrationDate,
//                  opt => opt.MapFrom(model => model.RegistrationDate ?? DateTime.Now))
//                 .ForMember(dto => dto.Id,
//                  opt => opt.MapFrom(model => model.Id ?? Guid.NewGuid()))
//                 .ForMember(dto => dto.PasswordHash,
//                  opt => opt.MapFrom(model => model.Password));

//            CreateMap<UserModel, UserDto>();
//            CreateMap<CustomerModel, CustomerDto>();
//            CreateMap<DoctorModel, DoctorDto>();
//        }
//    }
//}
