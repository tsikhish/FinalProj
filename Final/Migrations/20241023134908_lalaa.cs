using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class lalaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoggingAttempts",
                table: "AppUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlockedForFailedAttempts",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoggingAttempts",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsBlockedForFailedAttempts",
                table: "AppUsers");
        }
    }
}
