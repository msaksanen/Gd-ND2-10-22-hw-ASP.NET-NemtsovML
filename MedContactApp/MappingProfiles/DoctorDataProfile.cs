using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class DoctorDataProfile : Profile
    {
        public DoctorDataProfile()
        {
            CreateMap<DoctorData, DoctorDataDto>();

            CreateMap<DoctorDataDto, DoctorData>().ReverseMap();
        }
   
    }
}
