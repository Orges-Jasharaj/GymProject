using System.ComponentModel.DataAnnotations;

namespace GymProject.Dtos.Requests
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; }
    }
}
