using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class updated_mcms_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Judges",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteria_Order",
                table: "JudgingCriteria",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Order",
                table: "Categories",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_BasicPages_Order",
                table: "BasicPages",
                column: "Order");

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteria_Order",
                table: "JudgingCriteria");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Order",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_BasicPages_Order",
                table: "BasicPages");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Judges",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
