using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolModel.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManytoMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Professors_Courses");

            migrationBuilder.CreateTable(
                name: "ProfessorsCourses",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    ProfessorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessorsCourses", x => new { x.CoursesId, x.ProfessorsId });
                    table.ForeignKey(
                        name: "FK_ProfessorsCourses_Course_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Course",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessorsCourses_Professor_ProfessorsId",
                        column: x => x.ProfessorsId,
                        principalTable: "Professor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfessorsCourses_ProfessorsId",
                table: "ProfessorsCourses",
                column: "ProfessorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfessorsCourses");

            migrationBuilder.CreateTable(
                name: "Professors_Courses",
                columns: table => new
                {
                    ProfessorId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors_Courses", x => new { x.ProfessorId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_ProfessorsCourses_Course",
                        column: x => x.CourseId,
                        principalTable: "Professor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ProfessorsCourses_Professor",
                        column: x => x.ProfessorId,
                        principalTable: "Course",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Courses_CourseId",
                table: "Professors_Courses",
                column: "CourseId");
        }
    }
}
