using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class smtha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accountants_AccUserId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Accountants");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccUserId",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accountants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountants", x => x.Id);
                });

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
    }
}
