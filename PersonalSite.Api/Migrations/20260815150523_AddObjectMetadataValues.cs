using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonalSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectMetadataValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectMetadataValueEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectMetadataValueEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectMetadataValueEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ObjectMetadataValueId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    ValueType = table.Column<string>(type: "text", nullable: false),
                    ValueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectMetadataValueEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectMetadataValueEntry_ObjectMetadataValueEntity_ObjectMe~",
                        column: x => x.ObjectMetadataValueId,
                        principalTable: "ObjectMetadataValueEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectMetadataValueEntry_ObjectMetadataValueId",
                table: "ObjectMetadataValueEntry",
                column: "ObjectMetadataValueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectMetadataValueEntry");

            migrationBuilder.DropTable(
                name: "ObjectMetadataValueEntity");
        }
    }
}
