using MedContactCore.DataTransferObjects;
using System.ComponentModel.DataAnnotations;

namespace MedContactApp.Models
{
    public class AdminUserEditModel : BaseUserModel
    {
        public int[]? BlockStateIds { get; set; }
        public List<SelectItem>? BlockState { get; set; } = new List<SelectItem>()
        {
            new SelectItem(){ IntId=0, Name="Not Set"},
            new SelectItem(){ IntId=1, Name="False"},
            new SelectItem(){ IntId=2, Name="True"}
        };
        public List<RoleDto>? AllRoles { get; set; }
        public Guid[]? RoleIds { get; set; }
        public string? SystemInfo { get; set; }
        public string? Reflink { get; set; }
    }
}


