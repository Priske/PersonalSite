using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssistantKnowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssistantKnowledges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantKnowledges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssistantKnowledgeFiles",
                columns: table => new
                {
                    AssistantKnowledgeId = table.Column<int>(type: "integer", nullable: false),
                    StoredFileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssistantKnowledgeFiles", x => new { x.AssistantKnowledgeId, x.StoredFileId });
                    table.ForeignKey(
                        name: "FK_AssistantKnowledgeFiles_AssistantKnowledges_AssistantKnowle~",
                        column: x => x.AssistantKnowledgeId,
                        principalTable: "AssistantKnowledges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssistantKnowledgeFiles_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssistantKnowledgeFiles_StoredFileId",
                table: "AssistantKnowledgeFiles",
                column: "StoredFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssistantKnowledgeFiles");

            migrationBuilder.DropTable(
                name: "AssistantKnowledges");
        }
    }
}
