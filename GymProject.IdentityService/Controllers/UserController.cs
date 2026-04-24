using GymProject.Dtos.Requests;
using GymProject.Models;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = $"{RoleTypes.Admin},{RoleTypes.SuperAdmin}")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync(User);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = $"{RoleTypes.SuperAdmin},{RoleTypes.Admin}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserByIdAsync((id));
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{RoleTypes.SuperAdmin},{RoleTypes.Admin}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateUserAsync(id, updateUserDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{RoleTypes.SuperAdmin},{RoleTypes.Admin}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }

        [HttpPost("changepassword")]
        [Authorize(Roles = $"{RoleTypes.User},{RoleTypes.SuperAdmin},{RoleTypes.Admin}")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangeUserPassword(changePasswordDto);
            return Ok(result);
        }

        [HttpPut("ReactivateUser/{id}")]
        [Authorize(Roles = $"{RoleTypes.SuperAdmin}")]
        public async Task<IActionResult> ReactivateUserAsync(string id)
        {
            return Ok(await _userService.ReactivateUserAsync(id));
        }


    }
}
