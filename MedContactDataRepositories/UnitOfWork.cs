using MedContactDb;
using MedContactDb.Entities;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedContactContext _database;
        //public IBaseUserRepository<Customer> CustomerRepository { get; }
        public IRepository<User> UserRepository { get; }
        public IRepository<DoctorData> DoctorDataRepository { get; }
        //public IBaseUserRepository<Doctor> DoctorRepository { get; }
        public IRepository<DayTimeTable> DayTimeTableRepository { get; }
        public IRepository<Role> RoleRepository { get; }
        //public IRepository<RoleAllUser> RoleAllUserRepository { get; }


        public UnitOfWork(MedContactContext database,
               IRepository<User> userRepository, 
               IRepository<DayTimeTable> dayTimeTableRepository, 
               IRepository<Role> roleRepository, IRepository<DoctorData> doctorDataRepository)
        {
            _database = database;
            UserRepository = userRepository;
            DayTimeTableRepository = dayTimeTableRepository;
            DoctorDataRepository= doctorDataRepository;
            RoleRepository = roleRepository;
        }

        public async Task<int> Commit()
        {
            return await _database.SaveChangesAsync();
        }
    }
}