using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork 
{
    IBaseUserRepository<Customer> CustomerRepository { get; }
    IBaseUserRepository<User> UserRepository { get; }
    IBaseUserRepository<Doctor> DoctorRepository { get; }
    IRepository<DayTimeTable> DayTimeTableRepository { get; }

    Task<int> Commit();
}