﻿// <auto-generated />
using System;
using MedContactDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MedContactDb.Migrations
{
    [DbContext(typeof(MedContactContext))]
    [Migration("20221018082803_Families")]
    partial class Families
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AcsDataUser", b =>
                {
                    b.Property<Guid>("AcsDatasId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AcsDatasId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("AcsDataUser");
                });

            modelBuilder.Entity("MedContactDb.Entities.AcsData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsBlocked")
                        .HasColumnType("bit");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AcsDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("CustomerDataId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DayTimeTableId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CustomerDataId");

                    b.HasIndex("DayTimeTableId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("MedContactDb.Entities.CustomerData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsBlocked")
                        .HasColumnType("bit");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.ToTable("CustomerDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.DayTimeTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ConsultDuration")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DoctorDataId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("FinishWorkTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FreeTicketQty")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartWorkTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TotalTicketQty")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DoctorDataId");

                    b.ToTable("DayTimeTables");
                });

            modelBuilder.Entity("MedContactDb.Entities.DoctorData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsBlocked")
                        .HasColumnType("bit");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SpecialityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("SpecialityId");

                    b.HasIndex("UserId");

                    b.ToTable("DoctorDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.ExtraData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AcsDataId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsPropBlocked")
                        .HasColumnType("bit");

                    b.Property<int?>("PropIntValue")
                        .HasColumnType("int");

                    b.Property<string>("PropName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropStringValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AcsDataId");

                    b.ToTable("ExtraDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.Family", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MainUserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Families");
                });

            modelBuilder.Entity("MedContactDb.Entities.MedData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CustomerDataId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("InputDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ShortSummary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerDataId");

                    b.ToTable("MedData");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MedData");
                });

            modelBuilder.Entity("MedContactDb.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("MedContactDb.Entities.Speciality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("MedContactDb.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FamilyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDependent")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsFullBlocked")
                        .HasColumnType("bit");

                    b.Property<string>("MidName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleUser");
                });

            modelBuilder.Entity("MedContactDb.Entities.Recommendation", b =>
                {
                    b.HasBaseType("MedContactDb.Entities.MedData");

                    b.Property<Guid?>("DoctorDataId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("DoctorDataId");

                    b.HasDiscriminator().HasValue("Recommendation");
                });

            modelBuilder.Entity("AcsDataUser", b =>
                {
                    b.HasOne("MedContactDb.Entities.AcsData", null)
                        .WithMany()
                        .HasForeignKey("AcsDatasId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedContactDb.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MedContactDb.Entities.AcsData", b =>
                {
                    b.HasOne("MedContactDb.Entities.Role", "Role")
                        .WithMany("AcsDatas")
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("MedContactDb.Entities.Appointment", b =>
                {
                    b.HasOne("MedContactDb.Entities.CustomerData", "CustomerData")
                        .WithMany("Appointments")
                        .HasForeignKey("CustomerDataId");

                    b.HasOne("MedContactDb.Entities.DayTimeTable", "DayTimeTable")
                        .WithMany("Appointments")
                        .HasForeignKey("DayTimeTableId");

                    b.Navigation("CustomerData");

                    b.Navigation("DayTimeTable");
                });

            modelBuilder.Entity("MedContactDb.Entities.CustomerData", b =>
                {
                    b.HasOne("MedContactDb.Entities.Role", "Role")
                        .WithMany("CustomerDatas")
                        .HasForeignKey("RoleId");

                    b.HasOne("MedContactDb.Entities.User", "User")
                        .WithOne("CustomerData")
                        .HasForeignKey("MedContactDb.Entities.CustomerData", "UserId");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MedContactDb.Entities.DayTimeTable", b =>
                {
                    b.HasOne("MedContactDb.Entities.DoctorData", "DoctorData")
                        .WithMany("DayTimeTables")
                        .HasForeignKey("DoctorDataId");

                    b.Navigation("DoctorData");
                });

            modelBuilder.Entity("MedContactDb.Entities.DoctorData", b =>
                {
                    b.HasOne("MedContactDb.Entities.Role", "Role")
                        .WithMany("DoctorDatas")
                        .HasForeignKey("RoleId");

                    b.HasOne("MedContactDb.Entities.Speciality", "Speciality")
                        .WithMany("DoctorDatas")
                        .HasForeignKey("SpecialityId");

                    b.HasOne("MedContactDb.Entities.User", "User")
                        .WithMany("DoctorDatas")
                        .HasForeignKey("UserId");

                    b.Navigation("Role");

                    b.Navigation("Speciality");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MedContactDb.Entities.ExtraData", b =>
                {
                    b.HasOne("MedContactDb.Entities.AcsData", "AcsData")
                        .WithMany("ExtraDatas")
                        .HasForeignKey("AcsDataId");

                    b.Navigation("AcsData");
                });

            modelBuilder.Entity("MedContactDb.Entities.MedData", b =>
                {
                    b.HasOne("MedContactDb.Entities.CustomerData", "CustomerData")
                        .WithMany("MedData")
                        .HasForeignKey("CustomerDataId");

                    b.Navigation("CustomerData");
                });

            modelBuilder.Entity("MedContactDb.Entities.User", b =>
                {
                    b.HasOne("MedContactDb.Entities.Family", "Family")
                        .WithMany()
                        .HasForeignKey("FamilyId");

                    b.Navigation("Family");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("MedContactDb.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MedContactDb.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MedContactDb.Entities.Recommendation", b =>
                {
                    b.HasOne("MedContactDb.Entities.DoctorData", "DoctorData")
                        .WithMany()
                        .HasForeignKey("DoctorDataId");

                    b.Navigation("DoctorData");
                });

            modelBuilder.Entity("MedContactDb.Entities.AcsData", b =>
                {
                    b.Navigation("ExtraDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.CustomerData", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("MedData");
                });

            modelBuilder.Entity("MedContactDb.Entities.DayTimeTable", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("MedContactDb.Entities.DoctorData", b =>
                {
                    b.Navigation("DayTimeTables");
                });

            modelBuilder.Entity("MedContactDb.Entities.Role", b =>
                {
                    b.Navigation("AcsDatas");

                    b.Navigation("CustomerDatas");

                    b.Navigation("DoctorDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.Speciality", b =>
                {
                    b.Navigation("DoctorDatas");
                });

            modelBuilder.Entity("MedContactDb.Entities.User", b =>
                {
                    b.Navigation("CustomerData");

                    b.Navigation("DoctorDatas");
                });
#pragma warning restore 612, 618
        }
    }
}
