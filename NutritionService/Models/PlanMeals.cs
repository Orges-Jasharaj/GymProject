namespace GymProject.NutritionService.Models
{
    public class PlanMeals
    {
        public int Id { get; set; }
        public int NutritionPlanId { get; set; }
        public int MealId { get; set; }
        public TimeOnly TimeOfDay { get; set; }
        public double PortionSize { get; set; }
        public NutritionPlan NutritionPlan { get; set; }
        public Meals Meal { get; set; }
    }
}
