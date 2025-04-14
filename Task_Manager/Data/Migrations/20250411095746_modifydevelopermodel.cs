using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Manager.Data.Migrations
{
    /// <inheritdoc />
    public partial class modifydevelopermodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Developers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Developers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Developers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Developers");
        }
    }
}
