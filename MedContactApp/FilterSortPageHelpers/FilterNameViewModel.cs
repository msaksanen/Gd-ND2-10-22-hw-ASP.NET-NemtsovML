using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterNameViewModel
    {
        public FilterNameViewModel(string name, string email)
        {
            SelectedName = name;
            SelectedEmail = email;
        }
        public string SelectedName { get; }
        public string SelectedEmail { get; }

    }
}
