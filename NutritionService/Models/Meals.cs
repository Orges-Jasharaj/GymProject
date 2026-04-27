using System.ComponentModel.DataAnnotations;

namespace GymProject.NutritionService.Models
{
    public class Meals
    {
        public int Id { get; set; }
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
        public ICollection<PlanMeals> PlanMeals { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
