using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// DoctorInfoProfile
    /// </summary>
    public class DoctorInfoProfile : Profile
    {
        /// <summary>
        /// DoctorInfoProfile Ctor
        /// </summary>
        public DoctorInfoProfile()
        {
            CreateMap<(User, Speciality, DoctorData), DoctorInfo>()
                 .ForMember(info => info.Name, d => d.MapFrom(dt => dt.Item1.Name))
                 .ForMember(info => info.MidName, d => d.MapFrom(dt => dt.Item1.MidName))
                 .ForMember(info => info.Surname, d => d.MapFrom(dt => dt.Item1.Surname))
                 .ForMember(info => info.Speciality, d => d.MapFrom(dt => dt.Item2.Name))
                 .ForMember(info => info.ForDeletion, d => d.MapFrom(dt => dt.Item3.ForDeletion))
                 .ForMember(info => info.IsBlocked, d => d.MapFrom(dt => dt.Item3.IsBlocked))
                 .ForMember(info => info.SpecialityId, d => d.MapFrom(dt => dt.Item3.SpecialityId))
                 .ForMember(info => info.DoctorDataId, d => d.MapFrom(dt => dt.Item3.Id))
                 .ForMember(info => info.UserId, d => d.MapFrom(dt => dt.Item1.Id));

        }
    }
}
