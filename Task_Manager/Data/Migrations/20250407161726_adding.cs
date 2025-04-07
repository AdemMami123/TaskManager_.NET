using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Manager.Data.Migrations
{
    /// <inheritdoc />
    public partial class adding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Progress",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Tasks");

            migrationBuilder.AlterColumn<double>(
                name: "Progress",
                table: "Tasks",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
