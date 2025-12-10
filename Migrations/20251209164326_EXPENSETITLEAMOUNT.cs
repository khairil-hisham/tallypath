using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class EXPENSETITLEAMOUNT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Expenses",
                newName: "Title");

            migrationBuilder.AddColumn<long>(
                name: "Amount",
                table: "Expenses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Expenses",
                newName: "Content");
        }
    }
}
