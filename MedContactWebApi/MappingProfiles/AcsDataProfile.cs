using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// AcsDataProfile Mapping
    /// </summary>
    public class AcsDataProfile : Profile
    {
        /// <summary>
        /// AcsDataProfile Ctor
        /// </summary>
        public AcsDataProfile()
        {
            CreateMap<AcsData, AcsDataDto>();

            CreateMap<AcsDataDto, AcsData>().ReverseMap();
        }
   
    }
}
