using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaisyForum.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddViewCount_ToKnowledgeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "KnowledgeBases",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "KnowledgeBases");
        }
    }
}
