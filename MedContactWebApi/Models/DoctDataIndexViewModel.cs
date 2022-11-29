using MedContactWebApi.FilterSortPageHelpers;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// DoctDataIndexViewModel
    /// </summary>
    public class DoctDataIndexViewModel
    {
        ///
        public IEnumerable<DoctorFullDataDto> Users { get; }
        ///
        public PageViewModel PageViewModel { get; }
        ///
        public FilterSpecViewModel FilterViewModel { get; }
        ///
        public SortViewModel SortViewModel { get; }
        ///
        public string ProcessOptions { get; } = string.Empty;
        ///
        public string Reflink { get; } = string.Empty;
        
        /// <summary>
        /// DoctDataIndexViewModel Ctor
        /// </summary>
        /// <param name="users"></param>
        /// <param name="processOptions"></param>
        /// <param name="reflink"></param>
        /// <param name="pageViewModel"></param>
        /// <param name="filterViewModel"></param>
        /// <param name="sortViewModel"></param>
        public DoctDataIndexViewModel (IEnumerable<DoctorFullDataDto> users,
            string processOptions, string reflink, PageViewModel pageViewModel,
            FilterSpecViewModel filterViewModel, SortViewModel sortViewModel)
        {
            Users = users;
            ProcessOptions = processOptions;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            Reflink = reflink;
        }
    }
}
