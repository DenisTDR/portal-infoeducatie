using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class pages_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisplayTitle",
                table: "BasicPages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayTitle",
                table: "BasicPages");
        }
    }
}
