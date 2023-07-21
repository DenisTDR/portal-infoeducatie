using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class projects_participants_deletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Participants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Deleted",
                table: "Projects",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Deleted",
                table: "Participants",
                column: "Deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_Deleted",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Participants_Deleted",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Participants");
        }
    }
}
