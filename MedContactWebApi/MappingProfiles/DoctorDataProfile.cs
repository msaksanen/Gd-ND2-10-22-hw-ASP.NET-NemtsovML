using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// DoctorDataProfile
    /// </summary>
    public class DoctorDataProfile : Profile
    {
        /// <summary>
        /// DoctorDataProfile Ctor
        /// </summary>
        public DoctorDataProfile()
        {
            CreateMap<DoctorData, DoctorDataDto>();

            CreateMap<DoctorDataDto, DoctorData>().ReverseMap();
        }
   
    }
}
