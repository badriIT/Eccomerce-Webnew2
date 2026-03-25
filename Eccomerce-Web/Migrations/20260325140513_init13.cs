using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eccomerce_Web.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeCreatedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationAttempts",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeCreatedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "VerificationAttempts",
                table: "UserProfiles");
        }
    }
}
