using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{

    /// <summary>
    /// Role Mapping to RoleDto
    /// </summary>
    public class RoleProfile : Profile
    {

        /// <summary>
        /// Role Mapping to RoleDto Profile
        /// </summary>
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();

            CreateMap<RoleDto, Role>().ReverseMap();
        }
   
    }
}
