using MedContactWebApi.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.WebApi
{
    /// <summary>
    /// AppointmentIndexViewModel
    /// </summary>
    public class AppointmentIndexViewModel
    {
        /// 
        public IEnumerable<AppointmentInfo>? Appointments { get; }
        /// 
        public FilterAppointViewModel FilterViewModel { get; }
        /// 
        public SortViewModel SortViewModel { get; }
        /// 
        public Guid DayTimeTableId { get; set; } = Guid.Empty;
        /// 
        public Guid DoctorDataId { get; set; } = Guid.Empty;
        /// 
        public string SysInfo { get; } = string.Empty;
        ///
        public string Link { get; } = string.Empty;

        /// <summary>
        /// AppointmentIndexViewModel Ctor
        /// </summary>
        /// <param name="apms"></param>
        /// <param name="sysInfo"></param>
        /// <param name="link"></param>
        /// <param name="filterViewModel"></param>
        /// <param name="sortViewModel"></param>
        public AppointmentIndexViewModel(IEnumerable<AppointmentInfo>? apms, string sysInfo, string link,
         FilterAppointViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Appointments = apms;
            SysInfo = sysInfo;
            Link = link;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
