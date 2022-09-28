using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork
{
    IRepository<Customer> CustomerRepository { get; }

    Task<int> Commit();
}