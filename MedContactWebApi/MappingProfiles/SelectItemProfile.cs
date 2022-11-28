using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
	/// <summary>
	/// SelectItem Mapping from Role
	/// </summary>
	public class SelectItemProfile : Profile
    {
		/// <summary>
		/// SelectItem Mapping from Role Profile
		/// </summary>
		public SelectItemProfile()
        {

            CreateMap<RoleDto, SelectItem>()
		    .ForMember(s => s.guidId,
					opt => opt.MapFrom(r => r.Id))
			.ForMember(s => s.Name,
					opt => opt.MapFrom(r => r.Name));

			CreateMap<Role, SelectItem>()
            .ForMember(s => s.guidId,
					opt => opt.MapFrom(r => r.Id))
			.ForMember(s => s.Name,
					opt => opt.MapFrom(r => r.Name));


		}
   
    }
}
