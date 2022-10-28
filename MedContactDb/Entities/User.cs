using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedContactDb.Entities
{
    public  class User : IBaseEntity
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MidName { get; set; }
        public DateTime? BirthDate { get; set; }

        //public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public Guid? FamilyId { get; set; }
        public Family? Family { get; set; }
        public bool? IsDependent { get; set; }
        public bool? IsFullBlocked { get; set; }
        public List <DoctorData>? DoctorDatas { get; set; }
        public List<FileData>? FileDatas { get; set; }
        public CustomerData? CustomerData { get; set; }  //one-to-one
        public List <AcsData>? AcsDatas { get; set; }
        public List<Role>? Roles { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
