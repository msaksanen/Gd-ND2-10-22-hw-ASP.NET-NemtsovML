using MedContactDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedContactDb
{
    public class MedContactContext : DbContext
    {
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<Doctor>? Doctors { get; set; }
        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<RoleAllUser>? RoleAllUsers { get; set; }
        public DbSet<MedData>? MedData { get; set; }
        public DbSet<Recommendation>? Recommendations{ get; set; }
        public DbSet<Appointment>? Appointments { get; set; }
        public DbSet<DayTimeTable>? DayTimeTables { get; set; }

        public MedContactContext(DbContextOptions<MedContactContext> options)
        : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Role>()   //Course
                .HasMany(c => c.Customers) //Students
                .WithMany(s => s.Roles)   //Courses
                .UsingEntity<RoleAllUser>(   //Enrollment
                   j => j
                    .HasOne(pt => pt.Customer)   //Student
                    .WithMany(t => t.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.CustomerId), //StudentId
                j => j
                    .HasOne(pt => pt.Role) //Course
                    .WithMany(p => p.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.RoleId)); //CourseId
                //j =>
                //{
                //    j.HasKey(t => new { t.RoleId, t.CustomerId });
                //    j.ToTable("RoleAllUsers");
                //});
           
            modelBuilder
                .Entity<Role>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Roles)
                .UsingEntity<RoleAllUser>(   //Enrollment
                   j => j
                    .HasOne(pt => pt.User)   //Student
                    .WithMany(t => t.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.UserId), //StudentId
                j => j
                    .HasOne(pt => pt.Role) //Course
                    .WithMany(p => p.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.RoleId)); //CourseId
            
            modelBuilder
               .Entity<Role>()
               .HasMany(c => c.Doctors)
               .WithMany(s => s.Roles)
               .UsingEntity<RoleAllUser>(   //Enrollment
                   j => j
                    .HasOne(pt => pt.Doctor)   //Student
                    .WithMany(t => t.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.DoctorId), //StudentId
                j => j
                    .HasOne(pt => pt.Role) //Course
                    .WithMany(p => p.RoleAllUsers) //Enrollments
                    .HasForeignKey(pt => pt.RoleId)); //CourseId
        }

    }
}