using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactApp.MappingProfiles
{
    public class FileDataProfile : Profile
    {
        public FileDataProfile()
        {
            CreateMap<FileData, FileDataDto>();

            CreateMap<FileDataDto, FileData>().ReverseMap();
                  
        }

    }
}
