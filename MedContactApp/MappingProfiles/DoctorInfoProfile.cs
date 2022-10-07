using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class DoctorInfoProfile : Profile
    {
        public DoctorInfoProfile()
        {
            CreateMap<DoctorDto, DoctorInfo>();
            CreateMap<Doctor, DoctorInfo>();
        }
    }
}
                