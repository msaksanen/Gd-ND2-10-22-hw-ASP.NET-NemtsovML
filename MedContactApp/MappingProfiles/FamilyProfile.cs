using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class FamilyProfile : Profile
    {
        public FamilyProfile()
        {
            CreateMap<Family, FamilyDto>();

            CreateMap<FamilyDto, Family>().ReverseMap();
        }
   
    }
}
