using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterSpecViewModel
    {
        public FilterSpecViewModel(List<RoleDto> roles, Guid Id, string name, string email, string speciality)
        {
            roles.Insert(0, new RoleDto { Name = "All", Id = default });

            Roles = new SelectList(roles, "Id", "Name", Id);
            SelectedRole = Id;
            SelectedName = name;
            SelectedEmail = email;
            SelectedSpeciality = speciality;
        }
        public SelectList Roles { get; } 
        public Guid SelectedRole { get; } 
        public string SelectedName { get; }
        public string SelectedEmail { get; }
        public string SelectedSpeciality { get; }


    }
}
