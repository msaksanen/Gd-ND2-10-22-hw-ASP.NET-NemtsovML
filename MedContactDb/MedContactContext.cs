using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedContactDb
{
    public class MedContactContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<Family>? Families { get; set; }

        public DbSet<DoctorData>? DoctorDatas { get; set; }
        public DbSet<CustomerData>? CustomerDatas { get; set; }
        public DbSet<AcsData>? AcsDatas { get; set; }
        public DbSet<ExtraData>? ExtraDatas { get; set; }

        public DbSet<FileData>? FileDatas { get; set; }
        public DbSet<MedData>? MedData { get; set; }

        //public DbSet<Recommendation>? Recommendations { get; set; }
        public DbSet<Speciality>? Specialities { get; set; }
        public DbSet<Appointment>? Appointments { get; set; }
        public DbSet<DayTimeTable>? DayTimeTables { get; set; }
        public DbSet<RefreshToken>? RefreshTokens { get; set; }

        public MedContactContext(DbContextOptions<MedContactContext> options)
        : base(options)
        {
        }
        
    }
}