using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedContactDb.Migrations
{
    public partial class Initial_New : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MidName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFullBlocked = table.Column<bool>(type: "bit", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcsDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcsDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcsDatas_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerDatas_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DoctorDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true),
                    SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorDatas_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoctorDatas_Specialities_SpecialityId",
                        column: x => x.SpecialityId,
                        principalTable: "Specialities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DoctorDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcsDataUser",
                columns: table => new
                {
                    AcsDatasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcsDataUser", x => new { x.AcsDatasId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AcsDataUser_AcsDatas_AcsDatasId",
                        column: x => x.AcsDatasId,
                        principalTable: "AcsDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcsDataUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcsDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PropName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropStringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropIntValue = table.Column<int>(type: "int", nullable: true),
                    IsPropBlocked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraDatas_AcsDatas_AcsDataId",
                        column: x => x.AcsDataId,
                        principalTable: "AcsDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DayTimeTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartWorkTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishWorkTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsultDuration = table.Column<int>(type: "int", nullable: true),
                    TotalTicketQty = table.Column<int>(type: "int", nullable: true),
                    FreeTicketQty = table.Column<int>(type: "int", nullable: true),
                    DoctorDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayTimeTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayTimeTables_DoctorDatas_DoctorDataId",
                        column: x => x.DoctorDataId,
                        principalTable: "DoctorDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MedData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InputDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoctorDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedData_CustomerDatas_CustomerDataId",
                        column: x => x.CustomerDataId,
                        principalTable: "CustomerDatas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedData_DoctorDatas_DoctorDataId",
                        column: x => x.DoctorDataId,
                        principalTable: "DoctorDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DayTimeTableId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_CustomerDatas_CustomerDataId",
                        column: x => x.CustomerDataId,
                        principalTable: "CustomerDatas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_DayTimeTables_DayTimeTableId",
                        column: x => x.DayTimeTableId,
                        principalTable: "DayTimeTables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcsDatas_RoleId",
                table: "AcsDatas",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AcsDataUser_UsersId",
                table: "AcsDataUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerDataId",
                table: "Appointments",
                column: "CustomerDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DayTimeTableId",
                table: "Appointments",
                column: "DayTimeTableId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDatas_RoleId",
                table: "CustomerDatas",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDatas_UserId",
                table: "CustomerDatas",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DayTimeTables_DoctorDataId",
                table: "DayTimeTables",
                column: "DoctorDataId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDatas_RoleId",
                table: "DoctorDatas",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDatas_SpecialityId",
                table: "DoctorDatas",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorDatas_UserId",
                table: "DoctorDatas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraDatas_AcsDataId",
                table: "ExtraDatas",
                column: "AcsDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MedData_CustomerDataId",
                table: "MedData",
                column: "CustomerDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MedData_DoctorDataId",
                table: "MedData",
                column: "DoctorDataId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcsDataUser");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "ExtraDatas");

            migrationBuilder.DropTable(
                name: "MedData");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "DayTimeTables");

            migrationBuilder.DropTable(
                name: "AcsDatas");

            migrationBuilder.DropTable(
                name: "CustomerDatas");

            migrationBuilder.DropTable(
                name: "DoctorDatas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
