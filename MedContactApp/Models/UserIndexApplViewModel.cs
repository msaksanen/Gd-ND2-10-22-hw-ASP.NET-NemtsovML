using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class UserIndexApplViewModel
    {
        public IEnumerable<UserDto> Users { get; }
        public PageViewModel PageViewModel { get; }
        public FilterNameViewModel FilterNameViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string ProcessOptions { get; } = string.Empty;
        public UserIndexApplViewModel(IEnumerable<UserDto> users,
            string processOptions, PageViewModel pageViewModel,
            FilterNameViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Users = users;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterNameViewModel = filterViewModel;
            SortViewModel = sortViewModel;
        }
    }
}
