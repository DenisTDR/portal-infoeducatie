using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class projects_disabled_flag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Disabled",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Disabled",
                table: "Projects",
                column: "Disabled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Disabled",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "Projects");
        }
    }
}
