using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnIT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TutorLogo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Tutors",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Tutors");
        }
    }
}
