using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class smth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accountants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Accountants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
