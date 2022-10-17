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

            //from two dto in one model
            CreateMap<(DayTimeTableDto, DoctorInfo), DayTimeTableModel>()
                 .ForMember(model => model.Id, m => m.MapFrom(dto => dto.Item1.Id))
                 .ForMember(model => model.CreationDate, m => m.MapFrom(dto => dto.Item1.CreationDate))
                 .ForMember(model => model.Date, m => m.MapFrom(dto => dto.Item1.Date))
                 .ForMember(model => model.StartWorkTime, m => m.MapFrom(dto => dto.Item1.StartWorkTime))
                 .ForMember(model => model.FinishWorkTime, m => m.MapFrom(dto => dto.Item1.FinishWorkTime))
                 .ForMember(model => model.FinishWorkTime, m => m.MapFrom(dto => dto.Item1.FinishWorkTime))
                 .ForMember(model => model.ConsultDuration, m => m.MapFrom(dto => dto.Item1.ConsultDuration))
                 .ForMember(model => model.TotalTicketQty, m => m.MapFrom(dto => dto.Item1.TotalTicketQty))
                 .ForMember(model => model.FreeTicketQty, m => m.MapFrom(dto => dto.Item1.FreeTicketQty))
                 .ForMember(model => model.ConsultDuration, m => m.MapFrom(dto => dto.Item1.ConsultDuration))
                 .ForMember(model => model.DoctorName, m => m.MapFrom(doct => doct.Item2.Name))
                 .ForMember(model => model.DoctorMidName, m => m.MapFrom(doct => doct.Item2.MidName))
                 .ForMember(model => model.DoctorSurname, m => m.MapFrom(doct => doct.Item2.Surname))
                 .ForMember(model => model.DoctorSpeciality, m => m.MapFrom(doct => doct.Item2.Speciality));
        }
    }
}
