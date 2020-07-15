using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class seminars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Translation_Slug",
                table: "Translation");

            migrationBuilder.CreateTable(
                name: "Seminars",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Speaker = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    When = table.Column<DateTime>(nullable: false),
                    Published = table.Column<bool>(nullable: false),
                    Slug = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seminars", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translation_Slug",
                table: "Translation",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_Published",
                table: "Seminars",
                column: "Published");

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_Slug",
                table: "Seminars",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seminars");

            migrationBuilder.DropIndex(
                name: "IX_Translation_Slug",
                table: "Translation");

            migrationBuilder.CreateIndex(
                name: "IX_Translation_Slug",
                table: "Translation",
                column: "Slug");
        }
    }
}
