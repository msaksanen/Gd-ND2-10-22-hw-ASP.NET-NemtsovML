using Microsoft.EntityFrameworkCore;
using MedContactDb;
using MedContactDb.Entities;
using MedContactCore;
using MedContactCore.Abstractions;
using Microsoft.AspNetCore.Routing.Constraints;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MedContactWebApi.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using MedContactDataCQS.Tokens.Commands;
using MedContactWebApi.Controllers;
using MedContactWebApi.AdminPanelHelpers;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.SqlServer;

namespace MedContactWebApi
{
    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main Program method
        /// </summary>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //var myCorsPolicy = "myCors";
            builder.Host.UseSerilog((ctx, lc) =>
             lc.WriteTo.File(
                 @"D:\Logs\medcontact\webapi_data.log",
                 LogEventLevel.Information,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: null,
                  rollOnFileSizeLimit: true,
                  fileSizeLimitBytes: 4_194_304)
                 .WriteTo.Console(LogEventLevel.Verbose));

            // Add services to the container
            builder.Services.AddCors(options => options.AddPolicy("AllowLocalhost4200", 
                builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod())
               );
            //builder.Services.AddCors(options =>
            //{
            //   options.AddPolicy(myCorsPolicy, policyBuilder =>
            //   {
            //       policyBuilder
            //         .AllowAnyHeader()
            //         .AllowAnyMethod()
            //         .AllowAnyOrigin();
            //       //.WithOrigins("http://localhost:4200/");
            //   });
            //});

            builder.Services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            var connectionString = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<MedContactContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            builder.Services.AddHangfire(configuration => configuration
              .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
              .UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
              .UseSqlServerStorage(connectionString,
                  new SqlServerStorageOptions
                  {
                      CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                      SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                      QueuePollInterval = TimeSpan.Zero,
                      UseRecommendedIsolationLevel = true,
                      DisableGlobalLocks = true,
                  }));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

            builder.Services
               .AddAuthentication(options =>
               {
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(opt =>
               {
                   opt.RequireHttpsMetadata = false;
                   opt.SaveToken = true;
                   opt.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidIssuer = builder.Configuration["Token:Issuer"],
                       ValidAudience = builder.Configuration["Token:Issuer"],
                       IssuerSigningKey =
                           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:JwtSecret"])),
                       ClockSkew = TimeSpan.Zero
                   };
               });

            builder.Services.AddAuthorization(opts => {
                opts.AddPolicy("FullBlocked", policy => {
                    policy.RequireClaim("FullBlocked", "false");
                });
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMediatR(typeof(AddRefreshTokenCommand).Assembly);
            builder.Services.AddScoped<JWTSha256>();
            builder.Services.AddScoped<MD5>();
            builder.Services.AddScoped<DataChecker>();
            builder.Services.AddScoped<ModelUserBuilder>();
            builder.Services.AddScoped<AdminSortFilter>();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseRouting();
            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapHangfireDashboard();
           // app.UseCors(myCorsPolicy);
            app.UseCors("AllowLocalhost4200");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}