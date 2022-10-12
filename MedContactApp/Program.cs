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

            var connectionString = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<MedContactContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<IBaseUserRepository<Customer>, BaseUserRepository<Customer>>();
            builder.Services.AddScoped<IBaseUserRepository<User>, BaseUserRepository<User>>();
            builder.Services.AddScoped<IBaseUserRepository<Doctor>, BaseUserRepository<Doctor>>();

            builder.Services.AddScoped<IRepository<DayTimeTable>, Repository<DayTimeTable>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<RoleAllUser>, Repository<RoleAllUser>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<IRoleAllUserService<CustomerDto>, RoleAllUserService<CustomerDto, Customer>>();
            builder.Services.AddScoped<IRoleAllUserService<DoctorDto>, RoleAllUserService<DoctorDto, Doctor>>();
            builder.Services.AddScoped<IRoleAllUserService<UserDto>, RoleAllUserService<UserDto, User>>();

            builder.Services.AddScoped<IBaseUserService<CustomerDto>, BaseUserService<CustomerDto, Customer>>();
            builder.Services.AddScoped<IBaseUserService<UserDto>, BaseUserService<UserDto, User>>();
            builder.Services.AddScoped<IBaseUserService<DoctorDto>, BaseUserService<DoctorDto, Doctor>>();

            builder.Services.AddScoped<IDayTimeTableService, DayTimeTableService>();    
            builder.Services.AddScoped<EmailChecker<DoctorDto>>();



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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "customers",
                pattern: "{action}/{controller}/{page}",
                defaults: new { page = 0, controller = "Customer", action = "Index" },
                constraints: new { page = new IntRouteConstraint() });

            app.Run();
        }
    }
}