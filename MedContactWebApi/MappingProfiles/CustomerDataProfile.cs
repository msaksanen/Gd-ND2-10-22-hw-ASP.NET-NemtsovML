using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// CustomerDataProfile Mapping
    /// </summary>
    public class CustomerDataProfile : Profile
    {
        /// <summary>
        /// CustomerDataProfile Ctor
        /// </summary>
        public CustomerDataProfile()
        {
            CreateMap<CustomerData, CustomerDataDto>();

            CreateMap<CustomerDataDto, CustomerData>().ReverseMap();
        }
   
    }
}
