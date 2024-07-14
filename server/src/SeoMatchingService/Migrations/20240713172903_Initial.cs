using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeoMatchingService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "seo_rank_searched",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    searched_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    compared_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    search_engine = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    searched_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    seo_ranks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seo_rank_searched", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateIndex(
                name: "IX_seo_rank_searched_search_engine",
                table: "seo_rank_searched",
                column: "search_engine")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_seo_rank_searched_searched_at",
                table: "seo_rank_searched",
                column: "searched_at")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_seo_rank_searched_searched_at_search_engine",
                table: "seo_rank_searched",
                columns: new[] { "searched_at", "search_engine" })
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seo_rank_searched")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }
    }
}
