using Microsoft.EntityFrameworkCore;
using MedContactDb;
using MedContactDb.Entities;
using MedContactCore;
using System.ComponentModel.Design;
using MedContactBusiness.ServicesImplementations;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDataAbstractions;
using MedContactDataAbstractions.Repositories;
using MedContactDataRepositories;
using Microsoft.AspNetCore.Routing.Constraints;
using Serilog;
using Serilog.Events;
using MedContactApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MedContactApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Host.UseSerilog((ctx, lc) =>
               lc.WriteTo.File(
                   @"D:\Logs\medcontact\data.log",
                   LogEventLevel.Information)
                   .WriteTo.Console(LogEventLevel.Verbose));

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.LoginPath = new PathString(@"/Account/Login");
                    options.LogoutPath = new PathString(@"/Account/Logout");
                    //options.AccessDeniedPath = new PathString(@"/Account/Login");
                });

            var connectionString = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<MedContactContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Family>, Repository<Family>>();
     
            builder.Services.AddScoped<IRepository<DoctorData>, Repository<DoctorData>>();
            builder.Services.AddScoped<IRepository<Speciality>, Repository<Speciality>>();
            builder.Services.AddScoped<IRepository<DayTimeTable>, Repository<DayTimeTable>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
          
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRoleService, RoleService>();

           
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFamilyService, FamilyService>();
           
            builder.Services.AddScoped<IDoctorDataService, DoctorDataService>();
            builder.Services.AddScoped<ISpecialityService, SpecialityService>();
            builder.Services.AddScoped<IDayTimeTableService, DayTimeTableService>();    
            builder.Services.AddScoped<EmailChecker>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Set HttpContext.User
            app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{action}/{controller}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "customers",
                pattern: "{action}/{controller}/{page}",
                defaults: new { page = 0, controller = "Customer", action = "Index" },
                constraints: new { page = new IntRouteConstraint() });

            app.MapControllerRoute(
                name: "doctors",
                pattern: "{action}/{controller}/{page}",
                defaults: new { page = 0, controller = "Doctor", action = "Index" },
                constraints: new { page = new IntRouteConstraint() });

            app.Run();
        }
    }
}