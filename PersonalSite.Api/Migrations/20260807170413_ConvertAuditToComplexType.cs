using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAuditToComplexType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastEditedByUserId",
                table: "Tags",
                newName: "Edited_UserId");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "Tags",
                newName: "Edited_At");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Tags",
                newName: "Created_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tags",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "LastEditedByUserId",
                table: "Skills",
                newName: "Edited_UserId");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "Skills",
                newName: "Edited_At");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Skills",
                newName: "Created_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Skills",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "LastEditedByUserId",
                table: "SkillGroups",
                newName: "Edited_UserId");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "SkillGroups",
                newName: "Edited_At");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "SkillGroups",
                newName: "Created_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SkillGroups",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "LastEditedByUserId",
                table: "Projects",
                newName: "Edited_UserId");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "Projects",
                newName: "Edited_At");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Projects",
                newName: "Created_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Projects",
                newName: "Created_At");

            migrationBuilder.RenameColumn(
                name: "LastEditedByUserId",
                table: "HomepageConfigs",
                newName: "Edited_UserId");

            migrationBuilder.RenameColumn(
                name: "LastEditedAt",
                table: "HomepageConfigs",
                newName: "Edited_At");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "HomepageConfigs",
                newName: "Created_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "HomepageConfigs",
                newName: "Created_At");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Edited_UserId",
                table: "Tags",
                newName: "LastEditedByUserId");

            migrationBuilder.RenameColumn(
                name: "Edited_At",
                table: "Tags",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "Created_UserId",
                table: "Tags",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Tags",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Edited_UserId",
                table: "Skills",
                newName: "LastEditedByUserId");

            migrationBuilder.RenameColumn(
                name: "Edited_At",
                table: "Skills",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "Created_UserId",
                table: "Skills",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Skills",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Edited_UserId",
                table: "SkillGroups",
                newName: "LastEditedByUserId");

            migrationBuilder.RenameColumn(
                name: "Edited_At",
                table: "SkillGroups",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "Created_UserId",
                table: "SkillGroups",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "SkillGroups",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Edited_UserId",
                table: "Projects",
                newName: "LastEditedByUserId");

            migrationBuilder.RenameColumn(
                name: "Edited_At",
                table: "Projects",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "Created_UserId",
                table: "Projects",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Projects",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Edited_UserId",
                table: "HomepageConfigs",
                newName: "LastEditedByUserId");

            migrationBuilder.RenameColumn(
                name: "Edited_At",
                table: "HomepageConfigs",
                newName: "LastEditedAt");

            migrationBuilder.RenameColumn(
                name: "Created_UserId",
                table: "HomepageConfigs",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "HomepageConfigs",
                newName: "CreatedAt");
        }
    }
}
