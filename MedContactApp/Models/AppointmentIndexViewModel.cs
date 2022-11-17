using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class AppointmentIndexViewModel
    {
        public IEnumerable<AppointmentDto>? Appointments { get; }
        public FilterAppointViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public Guid DayTimeTableId { get; set; } = Guid.Empty;
        public Guid DoctorDataId { get; set; } = Guid.Empty;
        public string SysInfo { get; } = string.Empty;
        public AppointmentIndexViewModel(IEnumerable<AppointmentDto>? apms, string sysInfo,
         FilterAppointViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Appointments = apms;
            SysInfo = sysInfo;  
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
