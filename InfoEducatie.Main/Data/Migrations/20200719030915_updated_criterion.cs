using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class updated_criterion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "JudgingCriteria");

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "JudgingCriteria",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "JudgingCriteria");

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "JudgingCriteria",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
