using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePass.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class GranularVisitorDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostPerson",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehiclePlate",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisitPurpose",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "HostPerson",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "VehiclePlate",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "VisitPurpose",
                table: "Visitors");
        }
    }
}
