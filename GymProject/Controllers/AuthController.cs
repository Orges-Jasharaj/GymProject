using GymProject.Dtos.Requests;
using GymProject.Models;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IUser _userService;

        public AuthController(IUser userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);
            return Ok(result);
        }

        [HttpPost("registerUserWithRole")]
        [Authorize(Roles = $"{RoleTypes.SuperAdmin}")]
        public async Task<IActionResult> RegisterUserWithRole([FromBody] CreateUserDto createUserWithRoleDto, string role)
        {
            var result = await _userService.CreateUserWithRoleAsync(createUserWithRoleDto, role);
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            var result = await _userService.RefreshToken(refreshTokenDto);
            return Ok(result);
        }
    }
}
