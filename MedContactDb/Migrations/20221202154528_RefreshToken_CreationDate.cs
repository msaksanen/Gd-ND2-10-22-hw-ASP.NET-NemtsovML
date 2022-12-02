using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedContactDb.Migrations
{
    public partial class RefreshToken_CreationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "RefreshTokens");
        }
    }
}
