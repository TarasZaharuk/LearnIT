using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnIT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TutorSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop SkillTutor table if it exists
            migrationBuilder.DropTable(
                name: "SkillTutor");

            // Drop the Skills table
            migrationBuilder.DropTable(
                name: "Skills");

            // Recreate the Skills table
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TutorId = table.Column<int>(type: "int", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Clear Genders table before inserting new seed data
            migrationBuilder.Sql("DELETE FROM Genders");

            // Insert Genders data
            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { 1, "Male" },
            { 2, "Female" }
                });

            // Create index on TutorId in Skills table
            migrationBuilder.CreateIndex(
                name: "IX_Skills_TutorId",
                table: "Skills",
                column: "TutorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop Skills table
            migrationBuilder.DropTable(
                name: "Skills");

            // Remove Genders data
            migrationBuilder.Sql("DELETE FROM Genders WHERE Id IN (1, 2)");

            // Recreate SkillTutor table
            migrationBuilder.CreateTable(
                name: "SkillTutor",
                columns: table => new
                {
                    SkillsId = table.Column<int>(type: "int", nullable: false),
                    TutorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTutor", x => new { x.SkillsId, x.TutorId });
                    table.ForeignKey(
                        name: "FK_SkillTutor_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillTutor_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillTutor_TutorId",
                table: "SkillTutor",
                column: "TutorId");
        }
    }
}
