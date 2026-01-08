using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addKeyInPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "Pages");
        }
    }
}
