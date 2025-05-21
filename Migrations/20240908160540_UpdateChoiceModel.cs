using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEmplacementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChoiceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_Students_StudentId",
                table: "Choices");

            migrationBuilder.DropIndex(
                name: "IX_Choices_StudentId",
                table: "Choices");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Choices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Choices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Choices_StudentId",
                table: "Choices",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_Students_StudentId",
                table: "Choices",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
