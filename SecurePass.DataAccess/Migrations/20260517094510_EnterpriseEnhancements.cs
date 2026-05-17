using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePass.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExitDate",
                table: "Visitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInside",
                table: "Visitors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitDate",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IsInside",
                table: "Visitors");
        }
    }
}
