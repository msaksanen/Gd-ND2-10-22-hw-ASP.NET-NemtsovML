using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class CustomerDataProfile : Profile
    {
        public CustomerDataProfile()
        {
            CreateMap<CustomerData, CustomerDataDto>();

            CreateMap<CustomerDataDto, CustomerData>().ReverseMap();
        }
   
    }
}
