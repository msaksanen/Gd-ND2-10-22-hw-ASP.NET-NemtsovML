using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class MedDataIndexViewModel
    {
        public IEnumerable<MedDataInfo>? MedDatas { get; }
        public PageViewModel PageViewModel { get; }
        public FilterMedDataViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string ProcessOptions { get; } = string.Empty;
        public string SysInfo { get; } = string.Empty;
        public string Reflink { get; } = string.Empty;
        public string DoctId { get; set; } = string.Empty;
        public UserDto User { get; }

        public MedDataIndexViewModel(UserDto user, string sysinfo, string link, IEnumerable<MedDataInfo>? medDatas,
            string processOptions,PageViewModel pageViewModel,
            FilterMedDataViewModel filterViewModel, SortViewModel sortViewModel)
        {
            MedDatas = medDatas;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            User = user;
            SysInfo = sysinfo;
            Reflink = link;
        }
    }
}
