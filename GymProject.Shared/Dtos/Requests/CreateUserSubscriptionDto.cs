using System;
using System.ComponentModel.DataAnnotations;

namespace GymProject.Shared.Dtos.Requests
{
    public class CreateUserSubscriptionDto
    {
        public string? UserId { get; set; }

        [Required]
        public Guid SubscriptionPlanId { get; set; }

        public DateTime? StartDate { get; set; }
    }
}
