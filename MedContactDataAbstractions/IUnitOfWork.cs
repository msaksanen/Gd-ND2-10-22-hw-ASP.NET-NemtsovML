using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork 
{
   
    IRepository<User> UserRepository { get; }
    IRepository<DoctorData> DoctorDataRepository { get; }
    IRepository<Speciality> SpecialityRepository { get; }
    IRepository<DayTimeTable> DayTimeTableRepository { get; }
    IRepository<Role> RoleRepository { get; }
    IRepository<Family> FamilyRepository { get; }

    Task<int> Commit();
}