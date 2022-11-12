using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterOpenSpecViewModel
    {
        public FilterOpenSpecViewModel(string name, string speciality)
        {
            SelectedName = name;
            SelectedSpeciality = speciality;
        }
        public string SelectedName { get; }
        public string SelectedSpeciality { get; }


    }
}
