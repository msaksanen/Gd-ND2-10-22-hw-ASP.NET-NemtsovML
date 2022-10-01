using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork 
{
    IBaseUserRepository<Customer> CustomerRepository { get; }
    IBaseUserRepository<User> UserRepository { get; }

    Task<int> Commit();
}