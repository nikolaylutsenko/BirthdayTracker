using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthdayTracker.Backend.Migrations
{
    public partial class CompanyOwnerNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyOwnerId",
                table: "Companies",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyOwnerId",
                table: "Companies",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
