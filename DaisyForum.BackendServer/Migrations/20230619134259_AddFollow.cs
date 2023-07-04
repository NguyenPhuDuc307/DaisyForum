using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DaisyForum.BackendServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfFollowers",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    OwnerUserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    FollowerId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => new { x.OwnerUserId, x.FollowerId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Followers");

            migrationBuilder.DropColumn(
                name: "NumberOfFollowers",
                table: "AspNetUsers");
        }
    }
}
