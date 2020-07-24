using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class judge_user_cascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_AspNetUsers_UserId",
                table: "Judges");

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_AspNetUsers_UserId",
                table: "Judges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_AspNetUsers_UserId",
                table: "Judges");

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_AspNetUsers_UserId",
                table: "Judges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
