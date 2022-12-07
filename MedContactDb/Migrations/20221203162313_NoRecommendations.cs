using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedContactDb.Migrations
{
    public partial class NoRecommendations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedData_DoctorDatas_DoctorDataId",
                table: "MedData");

            migrationBuilder.DropIndex(
                name: "IX_MedData_DoctorDataId",
                table: "MedData");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "MedData");

            migrationBuilder.DropColumn(
                name: "DoctorDataId",
                table: "MedData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "MedData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorDataId",
                table: "MedData",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedData_DoctorDataId",
                table: "MedData",
                column: "DoctorDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedData_DoctorDatas_DoctorDataId",
                table: "MedData",
                column: "DoctorDataId",
                principalTable: "DoctorDatas",
                principalColumn: "Id");
        }
    }
}
