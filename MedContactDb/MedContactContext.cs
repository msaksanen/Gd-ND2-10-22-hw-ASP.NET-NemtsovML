using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedContactDb
{
    public class MedContactContext : DbContext
    {
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Doctor>? Doctors { get; set; }
        public DbSet<MedData>? MedData { get; set; }
        public DbSet<Recommendation>? Recommendations{ get; set; }
        public DbSet<Appointment>? Appointments { get; set; }
        public DbSet<DayTimeTable>? DayTimeTables { get; set; }

        public MedContactContext(DbContextOptions<MedContactContext> options)
        : base(options)
        {
        }
    }
}