using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEmplacementApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    SecondaryScore = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    MajorId = table.Column<int>(type: "int", nullable: false),
                    UniId = table.Column<int>(type: "int", nullable: false),
                    EnterenceScore = table.Column<float>(type: "real", nullable: true),
                    NumOfPlaces = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false, computedColumnSql: "[UniId] * 100 + [MajorId]"),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => new { x.UniId, x.MajorId });
                    table.ForeignKey(
                        name: "FK_Choices_Majors_MajorId",
                        column: x => x.MajorId,
                        principalTable: "Majors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Choices_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Choices_Universities_UniId",
                        column: x => x.UniId,
                        principalTable: "Universities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentChoices",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    UniId = table.Column<int>(type: "int", nullable: false),
                    MajorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentChoices", x => new { x.StudentId, x.UniId, x.MajorId });
                    table.ForeignKey(
                        name: "FK_StudentChoices_Choices_UniId_MajorId",
                        columns: x => new { x.UniId, x.MajorId },
                        principalTable: "Choices",
                        principalColumns: new[] { "UniId", "MajorId" });
                    table.ForeignKey(
                        name: "FK_StudentChoices_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_MajorId",
                table: "Choices",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_StudentId",
                table: "Choices",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentChoices_UniId_MajorId",
                table: "StudentChoices",
                columns: new[] { "UniId", "MajorId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentChoices");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Universities");
        }
    }
}
