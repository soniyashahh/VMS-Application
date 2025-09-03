using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class VisitorDetailsNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CHACompany",
                table: "visitorsregistration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CHAFilePath",
                table: "visitorsregistration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CHALicense",
                table: "visitorsregistration",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CHACompany",
                table: "visitorsregistration");

            migrationBuilder.DropColumn(
                name: "CHAFilePath",
                table: "visitorsregistration");

            migrationBuilder.DropColumn(
                name: "CHALicense",
                table: "visitorsregistration");
        }
    }
}
