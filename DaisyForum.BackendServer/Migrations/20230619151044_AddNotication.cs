using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaisyForum.BackendServer.Migrations
{
    /// <inheritdoc />
    public partial class AddNotication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Notification",
                table: "Followers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notification",
                table: "Followers");
        }
    }
}
