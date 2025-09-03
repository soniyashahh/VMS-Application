using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VMSApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class RRequestDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_companys_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_designations_DesignationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_hosts_systemCodes_StatusId",
                table: "hosts");

            migrationBuilder.DropForeignKey(
                name: "FK_securityForms_visitorsregistration_VisitorID",
                table: "securityForms");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorBlacklists_visitorsregistration_visitorId",
                table: "visitorBlacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_AspNetUsers_userId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_systemCodes_StatusId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_visitertypes_VisitortypeId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_visitorPurposes_visitorpurposeId",
                table: "visitorsregistration");

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fromdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Todate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    visitorId = table.Column<int>(type: "int", nullable: false),
                    VehicleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemsDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    visitorpurposeId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_requests_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requests_companys_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companys",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requests_systemCodes_StatusId",
                        column: x => x.StatusId,
                        principalTable: "systemCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_requests_visitorPurposes_visitorpurposeId",
                        column: x => x.visitorpurposeId,
                        principalTable: "visitorPurposes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requests_visitorsregistration_visitorId",
                        column: x => x.visitorId,
                        principalTable: "visitorsregistration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requests_CompanyId",
                table: "requests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_requests_StatusId",
                table: "requests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_requests_userId",
                table: "requests",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_requests_visitorId",
                table: "requests",
                column: "visitorId");

            migrationBuilder.CreateIndex(
                name: "IX_requests_visitorpurposeId",
                table: "requests",
                column: "visitorpurposeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_companys_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "companys",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_designations_DesignationId",
                table: "AspNetUsers",
                column: "DesignationId",
                principalTable: "designations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_hosts_systemCodes_StatusId",
                table: "hosts",
                column: "StatusId",
                principalTable: "systemCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_securityForms_visitorsregistration_VisitorID",
                table: "securityForms",
                column: "VisitorID",
                principalTable: "visitorsregistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorBlacklists_visitorsregistration_visitorId",
                table: "visitorBlacklists",
                column: "visitorId",
                principalTable: "visitorsregistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_AspNetUsers_userId",
                table: "visitorsregistration",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration",
                column: "CompanyId",
                principalTable: "companys",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_systemCodes_StatusId",
                table: "visitorsregistration",
                column: "StatusId",
                principalTable: "systemCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_visitertypes_VisitortypeId",
                table: "visitorsregistration",
                column: "VisitortypeId",
                principalTable: "visitertypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_visitorPurposes_visitorpurposeId",
                table: "visitorsregistration",
                column: "visitorpurposeId",
                principalTable: "visitorPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_companys_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_designations_DesignationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_hosts_systemCodes_StatusId",
                table: "hosts");

            migrationBuilder.DropForeignKey(
                name: "FK_securityForms_visitorsregistration_VisitorID",
                table: "securityForms");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorBlacklists_visitorsregistration_visitorId",
                table: "visitorBlacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_AspNetUsers_userId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_systemCodes_StatusId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_visitertypes_VisitortypeId",
                table: "visitorsregistration");

            migrationBuilder.DropForeignKey(
                name: "FK_visitorsregistration_visitorPurposes_visitorpurposeId",
                table: "visitorsregistration");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_companys_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "companys",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_designations_DesignationId",
                table: "AspNetUsers",
                column: "DesignationId",
                principalTable: "designations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_hosts_systemCodes_StatusId",
                table: "hosts",
                column: "StatusId",
                principalTable: "systemCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_securityForms_visitorsregistration_VisitorID",
                table: "securityForms",
                column: "VisitorID",
                principalTable: "visitorsregistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorBlacklists_visitorsregistration_visitorId",
                table: "visitorBlacklists",
                column: "visitorId",
                principalTable: "visitorsregistration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_AspNetUsers_userId",
                table: "visitorsregistration",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_companys_CompanyId",
                table: "visitorsregistration",
                column: "CompanyId",
                principalTable: "companys",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_systemCodes_StatusId",
                table: "visitorsregistration",
                column: "StatusId",
                principalTable: "systemCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_visitertypes_VisitortypeId",
                table: "visitorsregistration",
                column: "VisitortypeId",
                principalTable: "visitertypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_visitorsregistration_visitorPurposes_visitorpurposeId",
                table: "visitorsregistration",
                column: "visitorpurposeId",
                principalTable: "visitorPurposes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
