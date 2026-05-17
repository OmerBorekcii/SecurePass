using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurePass.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class OverstayAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpectedDurationHours",
                table: "Visitors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedDurationHours",
                table: "Visitors");
        }
    }
}
