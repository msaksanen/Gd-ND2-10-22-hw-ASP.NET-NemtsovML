using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class DoctDataIndexViewModel
    {
        public IEnumerable<DoctorFullDataDto> Users { get; }
        public PageViewModel PageViewModel { get; }
        public FilterSpecViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string ProcessOptions { get; } = string.Empty;
        public DoctDataIndexViewModel (IEnumerable<DoctorFullDataDto> users,
            string processOptions, PageViewModel pageViewModel,
            FilterSpecViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Users = users;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
