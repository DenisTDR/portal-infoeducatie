using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoEducatie.Main.Data.Migrations
{
    /// <inheritdoc />
    public partial class updated_mcms_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "AspNetRoles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Created",
                table: "Translations",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Updated",
                table: "Translations",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationItems_Created",
                table: "TranslationItems",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationItems_Updated",
                table: "TranslationItems",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_Created",
                table: "Seminars",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Seminars_Updated",
                table: "Seminars",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Created",
                table: "Projects",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Updated",
                table: "Projects",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJudgingCriterionPoints_Created",
                table: "ProjectJudgingCriterionPoints",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJudgingCriterionPoints_Updated",
                table: "ProjectJudgingCriterionPoints",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Created",
                table: "Participants",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Updated",
                table: "Participants",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Created",
                table: "Languages",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Updated",
                table: "Languages",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteriaSections_Created",
                table: "JudgingCriteriaSections",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteriaSections_Updated",
                table: "JudgingCriteriaSections",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteria_Created",
                table: "JudgingCriteria",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_JudgingCriteria_Updated",
                table: "JudgingCriteria",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Judges_Created",
                table: "Judges",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Judges_Updated",
                table: "Judges",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Created",
                table: "Files",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Updated",
                table: "Files",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Created",
                table: "Categories",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Updated",
                table: "Categories",
                column: "Updated");

            migrationBuilder.CreateIndex(
                name: "IX_BasicPages_Created",
                table: "BasicPages",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_BasicPages_Updated",
                table: "BasicPages",
                column: "Updated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Translations_Created",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_Updated",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_TranslationItems_Created",
                table: "TranslationItems");

            migrationBuilder.DropIndex(
                name: "IX_TranslationItems_Updated",
                table: "TranslationItems");

            migrationBuilder.DropIndex(
                name: "IX_Seminars_Created",
                table: "Seminars");

            migrationBuilder.DropIndex(
                name: "IX_Seminars_Updated",
                table: "Seminars");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Created",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Updated",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJudgingCriterionPoints_Created",
                table: "ProjectJudgingCriterionPoints");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJudgingCriterionPoints_Updated",
                table: "ProjectJudgingCriterionPoints");

            migrationBuilder.DropIndex(
                name: "IX_Participants_Created",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Participants_Updated",
                table: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_Languages_Created",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_Updated",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteriaSections_Created",
                table: "JudgingCriteriaSections");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteriaSections_Updated",
                table: "JudgingCriteriaSections");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteria_Created",
                table: "JudgingCriteria");

            migrationBuilder.DropIndex(
                name: "IX_JudgingCriteria_Updated",
                table: "JudgingCriteria");

            migrationBuilder.DropIndex(
                name: "IX_Judges_Created",
                table: "Judges");

            migrationBuilder.DropIndex(
                name: "IX_Judges_Updated",
                table: "Judges");

            migrationBuilder.DropIndex(
                name: "IX_Files_Created",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_Updated",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Created",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Updated",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_BasicPages_Created",
                table: "BasicPages");

            migrationBuilder.DropIndex(
                name: "IX_BasicPages_Updated",
                table: "BasicPages");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "AspNetRoles");
        }
    }
}
