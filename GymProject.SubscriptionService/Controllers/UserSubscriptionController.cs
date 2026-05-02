using GymProject.Services.Interface;
using GymProject.Shared.Dtos.Requests;
using GymProject.SubscriptionService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSubscriptionController : ControllerBase
    {
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserSubscriptionController(IUserSubscriptionService userSubscriptionService)
        {
            _userSubscriptionService = userSubscriptionService;
        }

        [HttpPost("SubscribeUser")]
        public async Task<IActionResult> SubscribeUser([FromBody] CreateUserSubscriptionDto createUserSubscriptionDto)
        {
            var result = await _userSubscriptionService.CreateUserSubscription(createUserSubscriptionDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetUserSubscriptions/{userId}")]
        public async Task<IActionResult> GetUserSubscriptions(string userId)
        {
            var result = await _userSubscriptionService.GetUserSubscriptionsByUserId(userId);
            return Ok(result);
        }

        [HttpGet("GetActiveSubscription/{userId}")]
        public async Task<IActionResult> GetActiveSubscription(string userId)
        {
            var result = await _userSubscriptionService.GetActiveUserSubscription(userId);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
