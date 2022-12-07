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
        public Guid UserId { get; } = Guid.Empty;

        public MedDataIndexViewModel(Guid userId, string sysinfo, IEnumerable<MedDataInfo>? medDatas,
            string processOptions,PageViewModel pageViewModel,
            FilterMedDataViewModel filterViewModel, SortViewModel sortViewModel)
        {
            MedDatas = medDatas;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            UserId = userId;
            SysInfo = sysinfo;
        }
    }
}
