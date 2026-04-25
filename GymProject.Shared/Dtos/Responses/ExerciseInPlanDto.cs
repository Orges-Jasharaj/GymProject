using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Shared.Dtos.Responses
{
    public class ExerciseInPlanDto
    {
        public Guid ExerciseId { get; set; }
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Order { get; set; }
    }
}
