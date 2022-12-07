using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterMedDataViewModel
    {
        public FilterMedDataViewModel(string name, string type, string speciality, 
                                      string depart, string text, string date)
        {
            SelectedName = name;
            SelectedType = type;
            SelectedSpec = speciality;
            SelectedDepart = depart;
            SelectedText = text;
            SelectedDate = date;
        }
        public string SelectedName { get; }
        public string SelectedType { get; }
        public string SelectedSpec { get; }
        public string SelectedDepart { get; }
        public string SelectedText { get; }
        public string SelectedDate { get; }
    }
}
