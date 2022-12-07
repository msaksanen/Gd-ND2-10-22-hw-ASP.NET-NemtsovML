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
        public IRepository<CustomerData> CustomerDataRepository { get; }
        public IRepository<Speciality> SpecialityRepository { get; }
        public IRepository<DayTimeTable> DayTimeTableRepository { get; }
        public IRepository<Role> RoleRepository { get; }              
        public IRepository<Family> FamilyRepository { get; }
        public IRepository<AcsData> AcsDataRepository { get; }
        public IRepository<ExtraData> ExtraDataRepository { get; }
        public IRepository<FileData> FileDataRepository { get; }
        public IRepository<MedData> MedDataRepository { get; }
        public IRepository<Appointment> AppointmentRepository { get; }


        public UnitOfWork(MedContactContext database,
               IRepository<User> userRepository, 
               IRepository<DayTimeTable> dayTimeTableRepository,
               IRepository<Role> roleRepository, IRepository<DoctorData> doctorDataRepository,
               IRepository<Family> familyRepository, IRepository<Speciality> specialityRepository,
               IRepository<AcsData> acsDataRepository, IRepository<ExtraData> extraDataRepository,
               IRepository<FileData> fileDataRepository, IRepository<Appointment> appointmentRepository,
               IRepository<CustomerData> customerDataRepository, IRepository<MedData> medDataRepository)
        {
            _database = database;
            UserRepository = userRepository;
            DayTimeTableRepository = dayTimeTableRepository;
            DoctorDataRepository = doctorDataRepository;
            RoleRepository = roleRepository;
            FamilyRepository = familyRepository;
            SpecialityRepository = specialityRepository;
            AcsDataRepository = acsDataRepository;
            ExtraDataRepository = extraDataRepository;
            FileDataRepository = fileDataRepository;
            AppointmentRepository = appointmentRepository;
            CustomerDataRepository = customerDataRepository;
            MedDataRepository = medDataRepository;
        }

        public async Task<int> Commit()
        {
            return await _database.SaveChangesAsync();
        }
    }
}