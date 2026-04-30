using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Shared.Dtos.Responses
{
    public class FitnessPlanDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ExerciseInPlanDto> Exercises { get; set; }
    }
}
