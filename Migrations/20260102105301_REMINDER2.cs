using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallypath.Migrations
{
    /// <inheritdoc />
    public partial class REMINDER2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reminder",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "hasReminder",
                table: "Contributions");

            migrationBuilder.AddColumn<string>(
                name: "Reminder",
                table: "SavingPlans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "hasReminder",
                table: "SavingPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reminder",
                table: "SavingPlans");

            migrationBuilder.DropColumn(
                name: "hasReminder",
                table: "SavingPlans");

            migrationBuilder.AddColumn<string>(
                name: "Reminder",
                table: "Contributions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "hasReminder",
                table: "Contributions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
