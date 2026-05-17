using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePass.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SecurityAndCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AgreementAccepted",
                table: "Visitors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlacklisted",
                table: "Visitors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVip",
                table: "Visitors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementAccepted",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IsBlacklisted",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "IsVip",
                table: "Visitors");
        }
    }
}
