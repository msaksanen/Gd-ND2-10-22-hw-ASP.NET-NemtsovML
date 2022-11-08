using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactApp.Models
{
    public class UserIndexViewModel
    {
        public IEnumerable<UserDto> Users { get; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string ProcessOptions { get; } = string.Empty;
        public GroupState GrpState { get; }
        public GroupState GrpCurrent { get; }
        public string? GroupIcon { get; } = string.Empty;
        public UserIndexViewModel (IEnumerable<UserDto> users,
            string processOptions, GroupState groupState, string icon, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Users = users;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            GrpCurrent = groupState;
            GrpState= groupState == GroupState.FamilyGroupOn ? GroupState.FamilyGroupOff : GroupState.FamilyGroupOn;
            GroupIcon = icon;
        }
    }
}
