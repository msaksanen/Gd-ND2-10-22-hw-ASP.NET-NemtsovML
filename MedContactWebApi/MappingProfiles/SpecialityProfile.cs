using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// SpecialityProfile
    /// </summary>
    public class SpecialityProfile : Profile
    {
        /// <summary>
        /// SpecialityProfile Ctor
        /// </summary>
        public SpecialityProfile()
        {
            CreateMap<Speciality, SpecialityDto>();

            CreateMap<SpecialityDto, Speciality>().ReverseMap();

           

        }
   
    }
}
