using System.ComponentModel.DataAnnotations;

namespace GymProject.Shared.Dtos.Requests
{
    public class UpdatePlanMealDto
    {
        [Required]
        public string DayOfWeek { get; set; }
        [Required]
        public string MealType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Calories { get; set; }
        [Required]
        public int Protein { get; set; }
        [Required]
        public int Carbohydrates { get; set; }
        [Required]
        public int Fats { get; set; }
    }
}
