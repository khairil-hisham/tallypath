using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class USERBALANCE2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBalances",
                table: "UserBalances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBalances",
                table: "UserBalances",
                column: "UserId");
        }
    }
}
