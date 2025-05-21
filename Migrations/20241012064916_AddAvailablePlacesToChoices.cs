using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEmplacementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailablePlacesToChoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailablePlaces",
                table: "Choices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailablePlaces",
                table: "Choices");
        }
    }
}
