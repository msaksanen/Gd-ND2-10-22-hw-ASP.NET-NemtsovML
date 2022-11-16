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
        public AppointmentIndexViewModel(IEnumerable<AppointmentDto>? apms,
            FilterAppointViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Appointments = apms;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
