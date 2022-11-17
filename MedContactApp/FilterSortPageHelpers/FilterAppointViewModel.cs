using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterAppointViewModel
    {
        public FilterAppointViewModel(string speciality, string name, string date, string birthdate)
        {
            SelectedName = name;
            SelectedSpeciality = speciality;
            SelectedDate = date;
            SelectedBirthDate = birthdate;  
    }

        public string SelectedName { get; }
        public string SelectedSpeciality { get; }
        public string SelectedDate { get; }
        public string SelectedBirthDate { get; }

    }
}
