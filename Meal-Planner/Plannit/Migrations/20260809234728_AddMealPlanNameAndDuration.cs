using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plannit.Migrations
{
    /// <inheritdoc />
    public partial class AddMealPlanNameAndDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "MealPlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MealPlans",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "MealPlans");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MealPlans");
        }
    }
}
