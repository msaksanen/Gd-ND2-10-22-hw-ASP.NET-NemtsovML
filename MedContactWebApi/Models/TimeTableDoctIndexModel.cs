using MedContactWebApi.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// TimeTableDoctIndexModel
    /// </summary>
    public class TimeTableDoctIndexModel
    {
        /// 
        public DoctorInfo DoctorInfo { get; set; }
        /// 
        public IEnumerable <DayTimeTableDto>? TableList { get; }
        /// 
        public PageViewModel PageViewModel { get; }
        /// 
        public SortViewModel SortViewModel { get; }
        ///
        public int Flag { get; set; } = 0;
        /// 
        public string ProcessOptions { get; } = string.Empty;
        /// 
        public string Reflink { get; set; } = string.Empty;

        /// <summary>
        /// TimeTableDoctIndexModel Ctor
        /// </summary>
        /// <param name="tableList"></param>
        /// <param name="processOptions"></param>
        /// <param name="doctorInfo"></param>
        /// <param name="reflink"></param>
        /// <param name="flag"></param>
        /// <param name="pageViewModel"></param>
        /// <param name="sortViewModel"></param>
        public TimeTableDoctIndexModel(IEnumerable<DayTimeTableDto>? tableList,
           string processOptions, DoctorInfo doctorInfo, string? reflink,  int flag,
           PageViewModel pageViewModel,SortViewModel sortViewModel)
        {
            TableList = tableList;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            SortViewModel = sortViewModel;
            DoctorInfo = doctorInfo;
            if (!string.IsNullOrEmpty(reflink))
                 Reflink = reflink;
            Flag = flag; 
        }
    }
}
