using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class threekeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccUserId",
                table: "Users",
                newName: "AccId");

            migrationBuilder.CreateTable(
                name: "Accountant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountant", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccId",
                table: "Users",
                column: "AccId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accountant_AccId",
                table: "Users",
                column: "AccId",
                principalTable: "Accountant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accountant_AccId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Accountant");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AccId",
                table: "Users",
                newName: "AccUserId");
        }
    }
}
