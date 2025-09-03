using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSafetyvideodata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitorSafeties");

            migrationBuilder.AddColumn<bool>(
                name: "Q1Answer",
                table: "SafetyVideo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Q2Answer",
                table: "SafetyVideo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Q3Answer",
                table: "SafetyVideo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Q4Answer",
                table: "SafetyVideo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Q5Answer",
                table: "SafetyVideo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Q1Answer",
                table: "SafetyVideo");

            migrationBuilder.DropColumn(
                name: "Q2Answer",
                table: "SafetyVideo");

            migrationBuilder.DropColumn(
                name: "Q3Answer",
                table: "SafetyVideo");

            migrationBuilder.DropColumn(
                name: "Q4Answer",
                table: "SafetyVideo");

            migrationBuilder.DropColumn(
                name: "Q5Answer",
                table: "SafetyVideo");

            migrationBuilder.CreateTable(
                name: "VisitorSafeties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emergency = table.Column<bool>(type: "bit", nullable: false),
                    PPE = table.Column<bool>(type: "bit", nullable: false),
                    SafetyVideo = table.Column<bool>(type: "bit", nullable: false),
                    Smoking = table.Column<bool>(type: "bit", nullable: false),
                    VisitorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorSafeties", x => x.Id);
                });
        }
    }
}
