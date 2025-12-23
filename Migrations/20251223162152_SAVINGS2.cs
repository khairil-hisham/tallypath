using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class SAVINGS2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "SavingPlans");

            migrationBuilder.AddColumn<DateTime>(
                name: "Due",
                table: "SavingPlans",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Due",
                table: "SavingPlans");

            migrationBuilder.AddColumn<string>(
                name: "Deadline",
                table: "SavingPlans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
