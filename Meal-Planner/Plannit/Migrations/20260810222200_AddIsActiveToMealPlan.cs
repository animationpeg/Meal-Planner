using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plannit.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToMealPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MealPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MealPlans");
        }
    }
}
