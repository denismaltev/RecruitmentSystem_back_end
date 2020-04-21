using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class AddLabourerAvailability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Availability",
                table: "Labourers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Labourers");
        }
    }
}
