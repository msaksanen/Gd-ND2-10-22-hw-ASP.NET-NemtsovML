using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactCore.DataTransferObjects
{
    public class BaseUserDto
    {
            public Guid Id { get; set; }
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? PasswordHash { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string? MidName { get; set; }
            public int? Age { get; set; }
            public string? Sex { get; set; }
            public string? Address { get; set; }
            public string? Role { get; set; }
            public DateTime RegistrationDate { get; set; }
       
    }
}