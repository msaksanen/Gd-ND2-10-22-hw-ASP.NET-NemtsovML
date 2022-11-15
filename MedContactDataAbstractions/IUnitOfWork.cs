using MedContactDb.Entities;
using MedContactDataAbstractions.Repositories;

namespace MedContactDataAbstractions;

public interface IUnitOfWork 
{
   
    IRepository<User> UserRepository { get; }
    IRepository<DoctorData> DoctorDataRepository { get; }
    IRepository<CustomerData> CustomerDataRepository { get; }
    IRepository<Speciality> SpecialityRepository { get; }
    IRepository<DayTimeTable> DayTimeTableRepository { get; }
    IRepository<Role> RoleRepository { get; }
    IRepository<Family> FamilyRepository { get; }
    IRepository<AcsData> AcsDataRepository { get; }
    IRepository<ExtraData> ExtraDataRepository { get; }
    IRepository<FileData> FileDataRepository { get; }
    IRepository<Appointment> AppointmentRepository { get; }

    Task<int> Commit();
}