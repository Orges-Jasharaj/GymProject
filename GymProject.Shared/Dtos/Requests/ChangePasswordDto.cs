using System.ComponentModel.DataAnnotations;

namespace GymProject.Dtos.Requests
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        public string? UserId { get; set; }
    }
}
