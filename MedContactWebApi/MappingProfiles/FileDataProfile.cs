using AutoMapper;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
namespace MedContactWebApi.MappingProfiles
{
    /// <summary>
    /// FileDataProfile Mapping
    /// </summary>
    public class FileDataProfile : Profile
    {
        /// <summary>
        /// FileDataProfile Ctor
        /// </summary>
        public FileDataProfile()
        {
            CreateMap<FileData, FileDataDto>();

            CreateMap<FileDataDto, FileData>().ReverseMap();
                  
        }

    }
}
