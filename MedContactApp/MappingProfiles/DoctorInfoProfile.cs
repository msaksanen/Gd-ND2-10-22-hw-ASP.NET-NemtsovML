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
            CreateMap<(User, Speciality, bool), DoctorInfo>()
                 .ForMember(info => info.Name, d => d.MapFrom(dt => dt.Item1.Name))
                 .ForMember(info => info.MidName, d => d.MapFrom(dt => dt.Item1.MidName))
                 .ForMember(info => info.Surname, d => d.MapFrom(dt => dt.Item1.Surname))
                 .ForMember(info => info.Speciality, d => d.MapFrom(dt => dt.Item2.Name))
                 .ForMember(info => info.ForDeletion, d => d.MapFrom(dt => dt.Item3));

        }
    }
}
