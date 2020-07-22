using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class updated_projects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Projects_ProjectId",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_ProjectId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Participants");

            migrationBuilder.AddColumn<string>(
                name: "DiscourseUrl",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscourseUrl",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "ProjectId",
                table: "Participants",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ProjectId",
                table: "Participants",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Projects_ProjectId",
                table: "Participants",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
