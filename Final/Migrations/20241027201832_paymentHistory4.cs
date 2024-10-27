using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class paymentHistory4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "Payment",
                newName: "PaidAmountForThisTime");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainedAmount",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidAmountForThisTime",
                table: "Payment",
                newName: "PaidAmount");

            migrationBuilder.AlterColumn<int>(
                name: "RemainedAmount",
                table: "Payment",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
