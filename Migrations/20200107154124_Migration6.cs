using Microsoft.EntityFrameworkCore.Migrations;

namespace CVEditor.Migrations
{
    public partial class Migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "NameId");

            migrationBuilder.AddColumn<string>(
                name: "NameId",
                table: "HRs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameId",
                table: "Admins",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameId",
                table: "HRs");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "NameId",
                table: "Users",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                nullable: false,
                defaultValue: "");
        }
    }
}
