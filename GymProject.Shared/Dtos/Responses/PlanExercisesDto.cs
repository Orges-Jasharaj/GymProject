namespace GymProject.Dtos.Responses
{
    public class PlanExercisesDto
    {
        public Guid Id { get; set; }
        public Guid FitnessPlanId { get; set; }
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int ExerciseOrder { get; set; }
        public string DayOfWeek { get; set; }
        public string Focus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
