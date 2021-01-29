using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InfoEducatie.Main.Data.Migrations
{
    public partial class updated_mcms_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectParticipants_Participants_ParticipantId",
                table: "ProjectParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectParticipants_Projects_ProjectId",
                table: "ProjectParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationItem_Language_LanguageId",
                table: "TranslationItem");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationItem_Translation_TranslationId",
                table: "TranslationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectParticipants",
                table: "ProjectParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ProjectParticipants_ParticipantId",
                table: "ProjectParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ProjectParticipants_ParticipantId_ProjectId",
                table: "ProjectParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslationItem",
                table: "TranslationItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translation",
                table: "Translation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                table: "Language");

            migrationBuilder.DropPrimaryKey(
                name: "PK_File",
                table: "File");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProjectParticipants");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ProjectParticipants");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "ProjectParticipants");

            migrationBuilder.RenameTable(
                name: "TranslationItem",
                newName: "TranslationItems");

            migrationBuilder.RenameTable(
                name: "Translation",
                newName: "Translations");

            migrationBuilder.RenameTable(
                name: "Language",
                newName: "Languages");

            migrationBuilder.RenameTable(
                name: "File",
                newName: "Files");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "ProjectParticipants",
                newName: "ProjectsId");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "ProjectParticipants",
                newName: "ParticipantsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectParticipants_ProjectId",
                table: "ProjectParticipants",
                newName: "IX_ProjectParticipants_ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_TranslationItem_TranslationId",
                table: "TranslationItems",
                newName: "IX_TranslationItems_TranslationId");

            migrationBuilder.RenameIndex(
                name: "IX_TranslationItem_LanguageId",
                table: "TranslationItems",
                newName: "IX_TranslationItems_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Translation_Slug",
                table: "Translations",
                newName: "IX_Translations_Slug");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Judges",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectParticipants",
                table: "ProjectParticipants",
                columns: new[] { "ParticipantsId", "ProjectsId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslationItems",
                table: "TranslationItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translations",
                table: "Translations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                table: "Languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectParticipants_Participants_ParticipantsId",
                table: "ProjectParticipants",
                column: "ParticipantsId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectParticipants_Projects_ProjectsId",
                table: "ProjectParticipants",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationItems_Languages_LanguageId",
                table: "TranslationItems",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationItems_Translations_TranslationId",
                table: "TranslationItems",
                column: "TranslationId",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectParticipants_Participants_ParticipantsId",
                table: "ProjectParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectParticipants_Projects_ProjectsId",
                table: "ProjectParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationItems_Languages_LanguageId",
                table: "TranslationItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationItems_Translations_TranslationId",
                table: "TranslationItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectParticipants",
                table: "ProjectParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translations",
                table: "Translations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslationItems",
                table: "TranslationItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                table: "Languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Translations",
                newName: "Translation");

            migrationBuilder.RenameTable(
                name: "TranslationItems",
                newName: "TranslationItem");

            migrationBuilder.RenameTable(
                name: "Languages",
                newName: "Language");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "File");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "ProjectParticipants",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "ParticipantsId",
                table: "ProjectParticipants",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectParticipants_ProjectsId",
                table: "ProjectParticipants",
                newName: "IX_ProjectParticipants_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Translations_Slug",
                table: "Translation",
                newName: "IX_Translation_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_TranslationItems_TranslationId",
                table: "TranslationItem",
                newName: "IX_TranslationItem_TranslationId");

            migrationBuilder.RenameIndex(
                name: "IX_TranslationItems_LanguageId",
                table: "TranslationItem",
                newName: "IX_TranslationItem_LanguageId");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ProjectParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ProjectParticipants",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ProjectParticipants",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Judges",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectParticipants",
                table: "ProjectParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translation",
                table: "Translation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslationItem",
                table: "TranslationItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                table: "Language",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_File",
                table: "File",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipants_ParticipantId",
                table: "ProjectParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectParticipants_ParticipantId_ProjectId",
                table: "ProjectParticipants",
                columns: new[] { "ParticipantId", "ProjectId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Judges_Categories_CategoryId",
                table: "Judges",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectParticipants_Participants_ParticipantId",
                table: "ProjectParticipants",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectParticipants_Projects_ProjectId",
                table: "ProjectParticipants",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationItem_Language_LanguageId",
                table: "TranslationItem",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationItem_Translation_TranslationId",
                table: "TranslationItem",
                column: "TranslationId",
                principalTable: "Translation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
