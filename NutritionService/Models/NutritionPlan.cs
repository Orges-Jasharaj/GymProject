using GymProject.NutritionService.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymProject.NutritionService.Models
{
    public class NutritionPlan
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public GoalType Goal { get; set; }
        public double TotalCalories { get; set; }
        public ICollection<PlanMeals> PlanMeals { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
