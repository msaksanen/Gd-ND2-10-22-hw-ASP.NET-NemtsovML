using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class TimeTableDoctIndexModel
    {
        public DoctorInfo DoctorInfo { get; set; }
        public IEnumerable <DayTimeTableDto>? TableList { get; }
        public PageViewModel PageViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string ProcessOptions { get; } = string.Empty;
        public int Flag { get; } = 0;
        public string Reflink { get; set; } = string.Empty;

        public TimeTableDoctIndexModel(IEnumerable<DayTimeTableDto>? tableList,
           string processOptions, DoctorInfo doctorInfo, int flag , string? reflink, PageViewModel pageViewModel,SortViewModel sortViewModel)
        {
            TableList = tableList;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            SortViewModel = sortViewModel;
            DoctorInfo = doctorInfo;
            Flag = flag;
            if (!string.IsNullOrEmpty(reflink))
                 Reflink = reflink;
        }
    }
}
