using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolModel.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deptName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    deptCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentId = table.Column<int>(type: "int", nullable: false),
                    courseName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    credits = table.Column<int>(type: "int", nullable: false),
                    courseNumber = table.Column<int>(type: "int", nullable: false),
                    availability = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.id);
                    table.ForeignKey(
                        name: "FK_Course_Department",
                        column: x => x.departmentId,
                        principalTable: "Department",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Professor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departmentId = table.Column<int>(type: "int", nullable: false),
                    firstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    lastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    partTime = table.Column<bool>(type: "bit", nullable: false),
                    workloadStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professor", x => x.id);
                    table.ForeignKey(
                        name: "FK_Professor_Department",
                        column: x => x.departmentId,
                        principalTable: "Department",
                        principalColumn: "id");
                });

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
                name: "IX_Course_courseNumber_departmentId",
                table: "Course",
                columns: new[] { "courseNumber", "departmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_departmentId",
                table: "Course",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Professor_departmentId",
                table: "Professor",
                column: "departmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_Courses_CourseId",
                table: "Professors_Courses",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Professors_Courses");

            migrationBuilder.DropTable(
                name: "Professor");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
