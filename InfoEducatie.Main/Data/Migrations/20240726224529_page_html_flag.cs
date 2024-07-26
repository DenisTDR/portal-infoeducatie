using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    /// <inheritdoc />
    public partial class page_html_flag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPlainHtml",
                table: "BasicPages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPlainHtml",
                table: "BasicPages");
        }
    }
}
