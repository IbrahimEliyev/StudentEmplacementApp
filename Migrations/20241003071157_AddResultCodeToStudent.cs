using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEmplacementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddResultCodeToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResultCode",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultCode",
                table: "Students");
        }
    }
}
