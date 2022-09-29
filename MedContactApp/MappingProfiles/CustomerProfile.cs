using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {

            CreateMap<Customer, CustomerDto>();

            CreateMap<CustomerDto, Customer>().ReverseMap();

            CreateMap<CustomerModel, CustomerDto>()
                 .ForMember(dto => dto.Role,
                  opt => opt.MapFrom(model => "Customer"))
                 .ForMember(dto => dto.RegistrationDate,
                  opt => opt.MapFrom(model => DateTime.Now))
                 .ForMember(dto => dto.Id,
                  opt => opt.MapFrom(model => Guid.NewGuid()))
                 .ForMember(dto => dto.PasswordHash,
                  opt => opt.MapFrom(model => model.Password));
        }

    }
}
