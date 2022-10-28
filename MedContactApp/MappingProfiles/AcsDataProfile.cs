using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class AcsDataProfile : Profile
    {
        public AcsDataProfile()
        {
            CreateMap<AcsData, AcsDataDto>();

            CreateMap<AcsDataDto, AcsData>().ReverseMap();
        }
   
    }
}
