using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork 
{
   //IBaseUserRepository<Customer> CustomerRepository { get; }
    IRepository<User> UserRepository { get; }
    IRepository<DoctorData> DoctorDataRepository { get; }

    //IBaseUserRepository<Doctor> DoctorRepository { get; }
    IRepository<DayTimeTable> DayTimeTableRepository { get; }
    IRepository<Role> RoleRepository { get; }
    IRepository<Family> FamilyRepository { get; }
    //IRepository<RoleAllUser> RoleAllUserRepository { get; }

    Task<int> Commit();
}