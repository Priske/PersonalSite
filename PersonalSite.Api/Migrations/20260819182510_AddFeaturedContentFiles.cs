using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedContentFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeaturedContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Created_At = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Created_UserId = table.Column<int>(type: "integer", nullable: true),
                    Edited_At = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Edited_UserId = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeaturedContentTag",
                columns: table => new
                {
                    FeaturedContentId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedContentTag", x => new { x.FeaturedContentId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_FeaturedContentTag_FeaturedContents_FeaturedContentId",
                        column: x => x.FeaturedContentId,
                        principalTable: "FeaturedContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeaturedContentTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeaturedContentFiles",
                columns: table => new
                {
                    FeaturedContentId = table.Column<int>(type: "integer", nullable: false),
                    StoredFileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedContentFiles", x => new { x.FeaturedContentId, x.StoredFileId });
                    table.ForeignKey(
                        name: "FK_FeaturedContentFiles_FeaturedContents_FeaturedContentId",
                        column: x => x.FeaturedContentId,
                        principalTable: "FeaturedContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeaturedContentFiles_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedContentFiles_StoredFileId",
                table: "FeaturedContentFiles",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedContentTag_TagsId",
                table: "FeaturedContentTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_StorageKey",
                table: "StoredFiles",
                column: "StorageKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeaturedContentFiles");

            migrationBuilder.DropTable(
                name: "FeaturedContentTag");

            migrationBuilder.DropTable(
                name: "StoredFiles");

            migrationBuilder.DropTable(
                name: "FeaturedContents");
        }
    }
}
