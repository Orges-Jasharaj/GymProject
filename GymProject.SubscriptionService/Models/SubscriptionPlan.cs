using System.ComponentModel.DataAnnotations;

namespace GymProject.SubscriptionService.Models
{
    public class SubscriptionPlan
    {
        public Guid Id { get; set; }
        [Required]
        public Guid GymId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int DurationInDays { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
