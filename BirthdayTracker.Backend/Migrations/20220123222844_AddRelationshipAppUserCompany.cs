using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthdayTracker.Backend.Migrations
{
    public partial class AddRelationshipAppUserCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_AppUser_AppUserId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AppUserId",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Companies",
                newName: "CompanyOwnerId");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "AppUser",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_CompanyId",
                table: "AppUser",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUser_Companies_CompanyId",
                table: "AppUser",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUser_Companies_CompanyId",
                table: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_AppUser_CompanyId",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AppUser");

            migrationBuilder.RenameColumn(
                name: "CompanyOwnerId",
                table: "Companies",
                newName: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AppUserId",
                table: "Companies",
                column: "AppUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_AppUser_AppUserId",
                table: "Companies",
                column: "AppUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
