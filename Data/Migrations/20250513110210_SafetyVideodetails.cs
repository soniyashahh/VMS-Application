using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class SafetyVideodetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SafetyVideo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Q1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Q2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Q3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Q4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Q5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyVideo_companys_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companys",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SafetyVideo_CompanyId",
                table: "SafetyVideo",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SafetyVideo");
        }
    }
}
