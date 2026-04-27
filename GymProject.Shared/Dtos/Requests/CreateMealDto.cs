using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Shared.Dtos.Requests
{
    public class CreateMealDto
    {
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
