using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class PatientIndexViewModel
    {
        public IEnumerable<AppointmentDto>? Appointments { get; }
        public FilterAppointViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public DoctorInfo DoctorInfo{ get; set; }
        public string SysInfo { get; } = string.Empty;
        public PatientIndexViewModel(IEnumerable<AppointmentDto>? apms, string sysInfo,
         DoctorInfo doctorInfo,
         FilterAppointViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Appointments = apms;
            SysInfo = sysInfo;  
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            DoctorInfo = doctorInfo;    
        }
    }
}
