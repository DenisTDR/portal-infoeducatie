using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class criteria_sections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SectionId",
                table: "JudgingCriteria",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JudgingCriteriaSections",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgingCriteriaSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JudgingCriteriaSections_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteria_SectionId",
                table: "JudgingCriteria",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteriaSections_CategoryId",
                table: "JudgingCriteriaSections",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JudgingCriteria_JudgingCriteriaSections_SectionId",
                table: "JudgingCriteria",
                column: "SectionId",
                principalTable: "JudgingCriteriaSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JudgingCriteria_JudgingCriteriaSections_SectionId",
                table: "JudgingCriteria");

            migrationBuilder.DropTable(
                name: "JudgingCriteriaSections");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteria_SectionId",
                table: "JudgingCriteria");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "JudgingCriteria");
        }
    }
}
