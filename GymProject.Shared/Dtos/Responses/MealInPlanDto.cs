namespace GymProject.Shared.Dtos.Responses
{
    public class MealInPlanDto
    {
        public int PlanMealId { get; set; }
        public int MealId { get; set; }
        public string DayOfWeek { get; set; }
        public string MealType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbohydrates { get; set; }
        public int Fats { get; set; }
    }
}
