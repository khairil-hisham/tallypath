using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class DAILYTOTALNEW3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyTotals",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTotals");
        }
    }
}
