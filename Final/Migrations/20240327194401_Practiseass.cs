using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class Practiseass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccUserId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Accountants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccUserId",
                table: "Users",
                column: "AccUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accountants_AccUserId",
                table: "Users",
                column: "AccUserId",
                principalTable: "Accountants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accountants_AccUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accountants");
        }
    }
}
