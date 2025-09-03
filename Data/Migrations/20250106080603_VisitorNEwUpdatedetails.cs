using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class VisitorNEwUpdatedetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "visitorsregistration",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_visitorsregistration_CompanyId",
                table: "visitorsregistration",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration",
                column: "CompanyId",
                principalTable: "companys",
                principalColumn: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration");

            migrationBuilder.DropIndex(
                name: "IX_visitorsregistration_CompanyId",
                table: "visitorsregistration");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "visitorsregistration");
        }
    }
}
