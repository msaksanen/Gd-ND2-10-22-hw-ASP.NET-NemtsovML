using MedContactDb;
using MedContactDb.Entities;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedContactContext _database;
        public IRepository<Customer> CustomerRepository { get; }

        public UnitOfWork(MedContactContext database,
            IRepository<Customer> customerRepository)
        {
            _database = database;
            CustomerRepository = customerRepository;
        }

        public async Task<int> Commit()
        {
            return await _database.SaveChangesAsync();
        }
    }
}