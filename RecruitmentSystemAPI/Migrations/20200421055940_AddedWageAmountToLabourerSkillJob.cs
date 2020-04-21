using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class AddedWageAmountToLabourerSkillJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WageAmount",
                table: "LabourerSkillJobs",
                type: "decimal(18, 0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WageAmount",
                table: "LabourerSkillJobs");
        }
    }
}
