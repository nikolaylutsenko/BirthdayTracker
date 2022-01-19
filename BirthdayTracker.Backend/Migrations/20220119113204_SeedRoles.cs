using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthdayTracker.Backend.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0a26e36f-1626-4298-9a97-34a8c4118e08", "0a26e36f-1626-4298-9a97-34a8c4118e08", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "58f3dea3-67eb-4284-b4bd-e4504d8e523e", "58f3dea3-67eb-4284-b4bd-e4504d8e523e", "Owner", "OWNER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dd62e685-29cf-4dd7-b59b-e44022d88d29", "dd62e685-29cf-4dd7-b59b-e44022d88d29", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0a26e36f-1626-4298-9a97-34a8c4118e08");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58f3dea3-67eb-4284-b4bd-e4504d8e523e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd62e685-29cf-4dd7-b59b-e44022d88d29");
        }
    }
}
