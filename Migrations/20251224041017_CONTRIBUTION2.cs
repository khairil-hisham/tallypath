using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class CONTRIBUTION2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contributions_SavingsId",
                table: "Contributions");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_SavingsId_CreatedAt",
                table: "Contributions",
                columns: new[] { "SavingsId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contributions_SavingsId_CreatedAt",
                table: "Contributions");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_SavingsId",
                table: "Contributions",
                column: "SavingsId");
        }
    }
}
