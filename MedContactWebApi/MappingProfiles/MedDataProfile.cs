using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// MedDataProfile Mapping
    /// </summary>
    public class MedDataProfile : Profile
    {
        /// <summary>
        /// MedDataProfile Ctor
        /// </summary>
        public MedDataProfile()
        {
            CreateMap<MedData, MedDataDto>();

            CreateMap<MedDataDto, MedData>().ReverseMap();
        }
   
    }
}
