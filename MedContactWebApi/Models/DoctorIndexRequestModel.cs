using MedContactWebApi.FilterSortPageHelpers;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// DoctorIndexRequestModel for Doctor Controller - Index
    /// </summary>
    public class DoctorIndexRequestModel
    {
       ///
       public string Name { get; set; } = string.Empty;
       ///
       public string Surname { get; set; } = string.Empty;
       ///
       public string Speciality { get; set; } = string.Empty;
       ///
       public int Page { get; set; } = 1;
       ///
       public SortState SortOrder { get; set; } = SortState.SpecialityAsc;
    }
}
