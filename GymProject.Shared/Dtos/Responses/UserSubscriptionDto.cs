using System;
using System.ComponentModel.DataAnnotations;

namespace GymProject.Shared.Dtos.Responses
{
    public class UserSubscriptionDto
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public Guid SubscriptionPlanId { get; set; }

        public string SubscriptionPlanName { get; set; } = null!;
        public Guid GymId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
