namespace GymProject.Dtos.Requests
{
    public class UpdateUserProfileRequest
    {
        public string? Gender { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? CurrentWeightKg { get; set; }
        public decimal? GoalWeightKg { get; set; }
        public string? ActivityLevel { get; set; }
        public string? FitnessGoal { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
