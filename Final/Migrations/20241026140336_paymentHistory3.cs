using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class paymentHistory3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RemainMonthlyPayment",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainMonthlyPayment",
                table: "Payment");
        }
    }
}
