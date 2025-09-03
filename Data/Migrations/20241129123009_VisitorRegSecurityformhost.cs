using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class VisitorRegSecurityformhost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "visitorsregistration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitorImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitortypeId = table.Column<int>(type: "int", nullable: false),
                    visitorpurposeId = table.Column<int>(type: "int", nullable: false),
                    UploadId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemsDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisitorLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleRegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrivingLicenceNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitorsregistration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_visitorsregistration_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_visitorsregistration_systemCodes_StatusId",
                        column: x => x.StatusId,
                        principalTable: "systemCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_visitorsregistration_visitertypes_VisitortypeId",
                        column: x => x.VisitortypeId,
                        principalTable: "visitertypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_visitorsregistration_visitorPurposes_visitorpurposeId",
                        column: x => x.visitorpurposeId,
                        principalTable: "visitorPurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "securityForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitorID = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_securityForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_securityForms_visitorsregistration_VisitorID",
                        column: x => x.VisitorID,
                        principalTable: "visitorsregistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_securityForms_VisitorID",
                table: "securityForms",
                column: "VisitorID");

            migrationBuilder.CreateIndex(
                name: "IX_visitorsregistration_StatusId",
                table: "visitorsregistration",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_visitorsregistration_userId",
                table: "visitorsregistration",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_visitorsregistration_visitorpurposeId",
                table: "visitorsregistration",
                column: "visitorpurposeId");

            migrationBuilder.CreateIndex(
                name: "IX_visitorsregistration_VisitortypeId",
                table: "visitorsregistration",
                column: "VisitortypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "securityForms");

            migrationBuilder.DropTable(
                name: "visitorsregistration");
        }
    }
}
