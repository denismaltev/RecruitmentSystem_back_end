using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class newFieldAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "QualityRating",
                table: "Labourers",
                unicode: false,
                maxLength: 450,
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SafetyRating",
                table: "Labourers",
                unicode: false,
                maxLength: 450,
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualityRating",
                table: "Labourers");

            migrationBuilder.DropColumn(
                name: "SafetyRating",
                table: "Labourers");
        }
    }
}
