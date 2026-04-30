using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymProject.NutritionService.Migrations
{
    /// <inheritdoc />
    public partial class AddMealPlanFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "PlanMeals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MealType",
                table: "PlanMeals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NutritionPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "PlanMeals");

            migrationBuilder.DropColumn(
                name: "MealType",
                table: "PlanMeals");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "NutritionPlans");
        }
    }
}
