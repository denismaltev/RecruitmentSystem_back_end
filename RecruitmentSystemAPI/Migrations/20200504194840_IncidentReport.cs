using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecruitmentSystemAPI.Migrations
{
    public partial class IncidentReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Summary = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReviewedByAdmin = table.Column<bool>(nullable: false),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Jobs",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabourerIncidentReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IncidentReportId = table.Column<int>(nullable: false),
                    LabourerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabourerIncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabourerIncidentReports_IncidentReports",
                        column: x => x.IncidentReportId,
                        principalTable: "IncidentReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Labourers",
                        column: x => x.LabourerId,
                        principalTable: "Labourers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_JobId",
                table: "IncidentReports",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_LabourerIncidentReports_IncidentReportId",
                table: "LabourerIncidentReports",
                column: "IncidentReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LabourerIncidentReports_LabourerId",
                table: "LabourerIncidentReports",
                column: "LabourerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabourerIncidentReports");

            migrationBuilder.DropTable(
                name: "IncidentReports");
        }
    }
}
