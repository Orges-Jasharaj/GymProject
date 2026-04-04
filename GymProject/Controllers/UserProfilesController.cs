using GymProject.Dtos.Requests;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfilesController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            return Ok(await _userProfileService.GetMyProfileAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserProfileRequest request)
        {
            return Ok(await _userProfileService.CreateAsync(request));  
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserProfileRequest request)
        {
            return Ok(await _userProfileService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _userProfileService.DeleteAsync(id));
        }
    }
}