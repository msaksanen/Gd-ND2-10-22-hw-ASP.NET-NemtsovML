using MedContactDb.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MedContactCore.DataTransferObjects
{
    public class UserDto
    {
            public Guid Id { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? PasswordHash { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string? MidName { get; set; }
            
            [DataType(DataType.Date)]   
            public DateTime? BirthDate { get; set; }

           // public int? Age { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public Guid? FamilyId { get; set; }
            public Guid? CustomerDataId { get; set; }
            public bool? IsOnline { get; set; }
            public bool? IsDependent { get; set; }
            public bool? IsFullBlocked { get; set; }
            public List<FileDataDto>? FileDatas { get; set; }
            public List<DoctorDataDto>? DoctorData { get; set; }
            public List<AcsDataDto>? AcsDatas { get; set; }
            public List<RoleDto>? Roles { get; set; }
            public DateTime RegistrationDate { get; set; }
            public DateTime LastLogin { get; set; }

    }
}