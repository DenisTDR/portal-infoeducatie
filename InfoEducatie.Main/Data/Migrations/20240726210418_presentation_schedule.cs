using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    /// <inheritdoc />
    public partial class presentation_schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PresentationSlotDuration",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PresentationSlotDuration",
                table: "Categories");
        }
    }
}
