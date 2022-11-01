using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterViewModel
    {
        public FilterViewModel(List<RoleDto> roles, Guid Id, string name)
        {
            roles.Insert(0, new RoleDto { Name = "All", Id = default });

            Roles = new SelectList(roles, "Id", "Name", Id);
            SelectedRole = Id;
            SelectedName = name;
        }
        public SelectList Roles { get; } 
        public Guid SelectedRole { get; } 
        public string SelectedName { get; }
        
    }
}
