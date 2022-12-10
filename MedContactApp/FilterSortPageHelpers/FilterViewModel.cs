using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MedContactApp.FilterSortPageHelpers
{
    public class FilterViewModel
    {
        public FilterViewModel(List<RoleDto> roles, Guid Id, string name, string email, string message, int count, string sysinfo="")
        {
            roles.Insert(0, new RoleDto { Name = "All", Id = default });

            Roles = new SelectList(roles, "Id", "Name", Id);
            SelectedRole = Id;
            SelectedName = name;
            SelectedEmail = email;

            if (string.IsNullOrEmpty(sysinfo))
                Message = message;
            else
                Message = string.Empty;
          
            SysInfo = sysinfo;
            if (!string.IsNullOrEmpty(SelectedEmail) ||
                  !string.IsNullOrEmpty(SelectedName) ||
                  SelectedRole != default)
                MessageCaption =$"Message for selected users ({count} persons)";
        }
        public SelectList Roles { get; } 
        public Guid SelectedRole { get; } 
        public string SelectedName { get; }
        public string SelectedEmail { get; }
        public string Message { get; }
        public string SysInfo { get; }
        public string MessageCaption { get; } = "Message for all users";
    }
}
