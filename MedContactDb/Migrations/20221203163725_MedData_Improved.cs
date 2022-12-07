using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedContactDb.Migrations
{
    public partial class MedData_Improved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "MedData");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorDataId",
                table: "MedData",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MedDataId",
                table: "FileDatas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedData_DoctorDataId",
                table: "MedData",
                column: "DoctorDataId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDatas_MedDataId",
                table: "FileDatas",
                column: "MedDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileDatas_MedData_MedDataId",
                table: "FileDatas",
                column: "MedDataId",
                principalTable: "MedData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedData_DoctorDatas_DoctorDataId",
                table: "MedData",
                column: "DoctorDataId",
                principalTable: "DoctorDatas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileDatas_MedData_MedDataId",
                table: "FileDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_MedData_DoctorDatas_DoctorDataId",
                table: "MedData");

            migrationBuilder.DropIndex(
                name: "IX_MedData_DoctorDataId",
                table: "MedData");

            migrationBuilder.DropIndex(
                name: "IX_FileDatas_MedDataId",
                table: "FileDatas");

            migrationBuilder.DropColumn(
                name: "DoctorDataId",
                table: "MedData");

            migrationBuilder.DropColumn(
                name: "MedDataId",
                table: "FileDatas");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "MedData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
