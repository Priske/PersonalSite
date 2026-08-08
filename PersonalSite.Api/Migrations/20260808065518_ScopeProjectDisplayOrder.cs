using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalSite.Api.Migrations
{
    /// <inheritdoc />
    public partial class ScopeProjectDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_DisplayOrder",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DisplayOrder",
                table: "Projects",
                column: "DisplayOrder",
                unique: true,
                filter: "\"Source\" = 0");
            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX "IX_Projects_DemoOwner_DisplayOrder"
                ON "Projects" ("Created_UserId", "DisplayOrder")
                WHERE "Source" = 1;
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX "IX_Projects_DemoOwner_DisplayOrder";
                """);

            migrationBuilder.DropIndex(
                name: "IX_Projects_DisplayOrder",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DisplayOrder",
                table: "Projects",
                column: "DisplayOrder",
                unique: true);
        }
    }
}
