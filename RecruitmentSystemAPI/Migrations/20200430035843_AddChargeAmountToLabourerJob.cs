using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class AddChargeAmountToLabourerJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChargeAmount",
                table: "LabourerJobs",
                type: "decimal(18, 0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeAmount",
                table: "LabourerJobs");
        }
    }
}
