using AutoMapper;
using MedContactApp.Models;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.MappingProfiles
{
    public class DayTimeTableProfile : Profile
    {
        public DayTimeTableProfile() 
        {
            CreateMap<DayTimeTable, DayTimeTableDto>();
            CreateMap<DayTimeTableDto, DayTimeTable>().ReverseMap();
            CreateMap<DayTimeTableModel, DayTimeTableDto>()
                 .ForMember(dto => dto.Id,
                  opt => opt.MapFrom(model => model.Id ?? Guid.NewGuid()));
        }
    }
}
