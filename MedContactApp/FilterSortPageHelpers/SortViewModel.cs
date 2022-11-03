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
        public SortState LastLoginSort { get; }
        public SortState IsFullBlockedSort { get; }
        public SortState IsFamilyDependentSort { get; }
        public SortState IsOnlineSort { get; }
        public SortState GenderSort { get; }
       
        public SortState Current { get; }
        public bool Up { get; set; }  

        public SortViewModel(SortState sortOrder)
        {
            EmailSort = SortState.EmailAsc;
            NameSort = SortState.NameAsc;
            SurnameSort = SortState.SurnameAsc;
            BirthDateSort = SortState.BirtDateAsc;
            RegDateSort = SortState.RegDateAsc;
            LastLoginSort = SortState.LastLoginAsc;
            GenderSort = SortState.GenderAsc;
            IsFullBlockedSort = SortState.IsFullBlockedAsc;
            IsFamilyDependentSort = SortState.IsFamilyDependentAsc;
            IsOnlineSort = SortState.IsOnlineAsc;


            Up = true;

            if (sortOrder == SortState.EmailDesc || sortOrder == SortState.BirthDateDesc || sortOrder == SortState.NameDesc
                || sortOrder == SortState.SurnameDesc || sortOrder == SortState.RegDateDesc || sortOrder==SortState.LastLoginDesc ||
                sortOrder == SortState.GenderDesc || sortOrder == SortState.IsOnlineDesc || sortOrder == SortState.IsFamilyDependentDesc ||
                sortOrder == SortState.IsFullBlockedDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortState.EmailDesc:
                    Current = EmailSort = SortState.EmailAsc;
                    break;
                case SortState.NameAsc:
                    Current = NameSort = SortState.NameDesc;
                    break;
                case SortState.NameDesc:
                    Current = NameSort = SortState.NameAsc;
                    break;
                case SortState.SurnameAsc:
                    Current = SurnameSort = SortState.SurnameDesc;
                    break;
                case SortState.SurnameDesc:
                    Current = SurnameSort = SortState.SurnameAsc;
                    break;
                case SortState.BirtDateAsc:
                    Current = BirthDateSort = SortState.BirthDateDesc;
                    break;
                case SortState.BirthDateDesc:
                    Current = BirthDateSort = SortState.BirtDateAsc;
                    break;
                case SortState.RegDateAsc:
                    Current = RegDateSort = SortState.RegDateDesc;
                    break;
                case SortState.RegDateDesc:
                    Current = RegDateSort = SortState.RegDateAsc;
                    break;
                case SortState.LastLoginAsc:
                    Current = LastLoginSort = SortState.LastLoginDesc;
                    break;
                case SortState.LastLoginDesc:
                    Current = LastLoginSort = SortState.LastLoginAsc;
                    break;
                case SortState.IsFullBlockedAsc:
                    Current = IsFullBlockedSort = SortState.IsFullBlockedDesc;
                    break;
                case SortState.IsFullBlockedDesc:
                    Current = IsFullBlockedSort = SortState.IsFullBlockedAsc;
                    break;
                case SortState.IsFamilyDependentAsc:
                    Current = IsFamilyDependentSort = SortState.IsFullBlockedDesc;
                    break;
                case SortState.IsFamilyDependentDesc:
                    Current = IsFamilyDependentSort = SortState.IsFamilyDependentAsc;
                    break;
                case SortState.IsOnlineAsc:
                    Current = IsOnlineSort = SortState.IsOnlineDesc;
                    break;
                case SortState.IsOnlineDesc:
                    Current = IsOnlineSort = SortState.IsOnlineAsc;
                    break;
                case SortState.GenderAsc:
                    Current = GenderSort = SortState.GenderDesc;
                    break;
                case SortState.GenderDesc:
                    Current = GenderSort = SortState.GenderAsc;
                    break;
                default:
                    Current = EmailSort = SortState.EmailDesc;
                    break;
            }
        }
    }
}
