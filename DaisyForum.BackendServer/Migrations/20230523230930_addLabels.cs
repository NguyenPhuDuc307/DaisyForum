using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaisyForum.BackendServer.Migrations
{
    /// <inheritdoc />
    public partial class addLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Labels",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Labels",
                table: "AspNetUsers");
        }
    }
}
