using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// ExtraDataProfile Mapping
    /// </summary>
    public class ExtraDataProfile : Profile
    {
        /// <summary>
        /// ExtraDataProfile Ctor
        /// </summary>
        public ExtraDataProfile()
        {
            CreateMap<ExtraData, ExtraDataDto>();

            CreateMap<ExtraDataDto, ExtraData>().ReverseMap();
        }
   
    }
}
