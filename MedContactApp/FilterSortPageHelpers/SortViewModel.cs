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
        public bool Up { get; set; }  



        //public SortViewModel(SortState sortOrder)
        //{
        //    EmailSort = sortOrder == SortState.EmailAsc ? SortState.EmailDesc : SortState.EmailAsc;
        //    NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
        //    SurnameSort = sortOrder == SortState.SurnameAsc ? SortState.SurnameDesc : SortState.SurnameAsc;
        //    BirthDateSort = sortOrder == SortState.BirtDateAsc ? SortState.BirthDateDesc : SortState.BirtDateAsc;
        //    RegDateSort = sortOrder == SortState.RegDateAsc ? SortState.RegDateDesc : SortState.RegDateAsc;
        //    Current = sortOrder;
        //}


        public SortViewModel(SortState sortOrder)
        {
            EmailSort = SortState.EmailAsc;
            NameSort = SortState.NameAsc;
            SurnameSort = SortState.SurnameAsc;
            BirthDateSort = SortState.BirtDateAsc;
            RegDateSort = SortState.RegDateAsc;
            Up = true;

            if (sortOrder == SortState.EmailDesc || sortOrder == SortState.BirthDateDesc || sortOrder == SortState.NameDesc
                || sortOrder == SortState.SurnameDesc || sortOrder == SortState.RegDateDesc)
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
                default:
                    Current = EmailSort = SortState.EmailDesc;
                    break;
            }
        }
    }
}
