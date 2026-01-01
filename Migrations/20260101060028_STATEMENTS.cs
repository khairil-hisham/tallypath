using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class STATEMENTS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStatement",
                table: "Expenses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStatement",
                table: "Expenses");
        }
    }
}
