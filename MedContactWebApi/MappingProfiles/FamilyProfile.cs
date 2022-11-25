using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// Family Mapping to FamilyDto
    /// </summary>
    public class FamilyProfile : Profile
    {
        /// <summary>
        /// Family Mapping to FamilyDto Profile
        /// </summary>
        public FamilyProfile()
        {
            CreateMap<Family, FamilyDto>();

            CreateMap<FamilyDto, Family>().ReverseMap();
        }
   
    }
}
