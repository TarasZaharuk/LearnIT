using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnIT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityStateForTutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntityState",
                table: "Tutors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityState",
                table: "Tutors");
        }
    }
}
