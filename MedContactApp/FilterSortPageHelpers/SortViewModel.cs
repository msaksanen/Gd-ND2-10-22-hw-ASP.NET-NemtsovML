using MedContactApp.FilterSortHelpers;

namespace MedContactApp.FilterSortPageHelpers
{
    public class SortViewModel
    {
        public SortState EmailSort { get; }
        public SortState NameSort { get; }
        public SortState SurnameSort { get; }
        public SortState BirthDateSort { get; }    
        public SortState RegDateSort { get; }
        public SortState Current { get; }


        public SortViewModel(SortState sortOrder)
        {
            EmailSort = sortOrder == SortState.EmailAsc ? SortState.EmailDesc : SortState.EmailAsc;
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            SurnameSort = sortOrder == SortState.SurnameAsc ? SortState.SurnameDesc : SortState.SurnameAsc;
            BirthDateSort = sortOrder == SortState.BirtDateAsc ? SortState.BirthDateDesc : SortState.BirtDateAsc;
            RegDateSort = sortOrder == SortState.RegDateAsc ? SortState.RegDateDesc : SortState.RegDateAsc;
            Current = sortOrder;
        }
    }
}
