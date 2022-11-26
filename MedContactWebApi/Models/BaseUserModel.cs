using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MedContactWebApi.Models
{
    /// <summary>
    /// BaseUserModel
    /// </summary>
    public class BaseUserModel
    {
        ///
        public Guid? Id { get; set; }
        ///
        public string? Email { get; set; }
        ///
        public string? PhoneNumber { get; set; }
        ///
        public string? Name { get; set; }
        ///
        public string? Surname { get; set; }
        ///
        public string? MidName { get; set; }
        ///
        public DateTime? BirthDate { get; set; }
        ///
        //public string? Password { get; set; }
        /////
        //public string? PasswordConfirmation { get; set; }
        /////
        public string? Gender { get; set; }
        ///
        public string? Address { get; set; }
        ///
        public Guid? FamilyId { get; set; }
        ///
        public Guid? CustomerDataId { get; set; }
        ///
        public bool? IsDependent { get; set; }
        ///
        public bool? IsFullBlocked { get; set; }
        ///
        public List<string>? RoleNames { get; set; }
        ///
        public DateTime? RegistrationDate { get; set; }
        ///
        public bool? IsOnline { get; set; }
        ///
        public DateTime LastLogin { get; set; }

    }
}
