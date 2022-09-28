using AutoMapper;
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

            //CreateMap<Customer, BaseUserDto>();

            //CreateMap<BaseUserDto, Customer>().ReverseMap();

        }

    }
}
