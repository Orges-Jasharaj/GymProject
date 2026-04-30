namespace GymProject.Shared.Dtos.Responses
{
    public class NutritionPlanDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MealInPlanDto> Meals { get; set; } = new();
    }
}
