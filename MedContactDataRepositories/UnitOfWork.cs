using MedContactDb;
using MedContactDb.Entities;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedContactContext _database;
        public IRepository<User> UserRepository { get; }
        public IRepository<DoctorData> DoctorDataRepository { get; }
        public IRepository<Speciality> SpecialityRepository { get; }
        public IRepository<DayTimeTable> DayTimeTableRepository { get; }
        public IRepository<Role> RoleRepository { get; }              
        public IRepository<Family> FamilyRepository { get; }


        public UnitOfWork(MedContactContext database,
               IRepository<User> userRepository, 
               IRepository<DayTimeTable> dayTimeTableRepository,
               IRepository<Role> roleRepository, IRepository<DoctorData> doctorDataRepository, 
               IRepository<Family> familyRepository, IRepository<Speciality> specialityRepository)
        {
            _database = database;
            UserRepository = userRepository;
            DayTimeTableRepository = dayTimeTableRepository;
            DoctorDataRepository = doctorDataRepository;
            RoleRepository = roleRepository;
            FamilyRepository = familyRepository;
            SpecialityRepository = specialityRepository;
        }

        public async Task<int> Commit()
        {
            return await _database.SaveChangesAsync();
        }
    }
}