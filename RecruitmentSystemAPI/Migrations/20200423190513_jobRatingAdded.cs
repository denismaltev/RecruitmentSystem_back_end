using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class jobRatingAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "JobRating",
                table: "Jobs",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobRating",
                table: "Jobs");
        }
    }
}
