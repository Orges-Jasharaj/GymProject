namespace GymProject.Dtos.Responses
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public string? Gender { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? CurrentWeightKg { get; set; }
        public decimal? GoalWeightKg { get; set; }
        public string? ActivityLevel { get; set; }
        public string? FitnessGoal { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
