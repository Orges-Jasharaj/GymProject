using System.ComponentModel.DataAnnotations;

namespace GymProject.Shared.Dtos.Requests
{
    public class CreateNutritionPlanDto
    {
        [Required]
        public string Name { get; set; }
    }
}
