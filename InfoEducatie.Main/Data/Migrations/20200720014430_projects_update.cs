using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class projects_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInOpen",
                table: "Projects",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInOpen",
                table: "Projects");
        }
    }
}
