using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class ExtraDataProfile : Profile
    {
        public ExtraDataProfile()
        {
            CreateMap<ExtraData, ExtraDataDto>();

            CreateMap<ExtraDataDto, ExtraData>().ReverseMap();
                  
        }

    }
}
