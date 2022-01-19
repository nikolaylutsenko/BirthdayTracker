using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthdayTracker.Backend.Migrations
{
    public partial class SeedIdentityUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0a26e36f-1626-4298-9a97-34a8c4118e08", "25d733fa-b5ce-41fe-a868-beea7723a3e5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0a26e36f-1626-4298-9a97-34a8c4118e08", "25d733fa-b5ce-41fe-a868-beea7723a3e5" });
        }
    }
}
