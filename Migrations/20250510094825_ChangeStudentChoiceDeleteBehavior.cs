using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEmplacementApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStudentChoiceDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChoices_Students_StudentId",
                table: "StudentChoices");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChoices_Students_StudentId",
                table: "StudentChoices",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentChoices_Students_StudentId",
                table: "StudentChoices");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentChoices_Students_StudentId",
                table: "StudentChoices",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
