using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymProject.Shared.Dtos.Requests
{
    public class CreateSubscriptionPlanDto
    {
        public Guid GymId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int DurationInDays { get; set; }
    }
}
