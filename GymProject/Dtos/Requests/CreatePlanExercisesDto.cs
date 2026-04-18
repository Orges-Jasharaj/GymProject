namespace GymProject.Dtos.Requests
{
    public class CreatePlanExercisesDto
    {
        public Guid FitnessPlanId { get; set; }
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int ExerciseOrder { get; set; }
    }
}
