using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class updated_participant_and_projects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ScoreOpen",
                table: "Projects",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ScoreProject",
                table: "Projects",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Cnp",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCardNumber",
                table: "Participants",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCardSeries",
                table: "Participants",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScoreOpen",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ScoreProject",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Cnp",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "IdCardNumber",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "IdCardSeries",
                table: "Participants");
        }
    }
}
