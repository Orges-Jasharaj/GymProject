using GymProject.Shared.Dtos.Requests;
using GymProject.SubscriptionService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlanController : Controller
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public SubscriptionPlanController(ISubscriptionPlanService subscriptionPlanService)
        {
            _subscriptionPlanService = subscriptionPlanService;
        }

        [HttpPost("CreateSubscriptionPlan")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanDto createSubscriptionPlanDto)
        {
            var result = await _subscriptionPlanService.CreateSubscriptionPlan(createSubscriptionPlanDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("UpdateSubscriptionPlan/{subscriptionPlanId}")]
        public async Task<IActionResult> UpdateSubscriptionPlan(Guid subscriptionPlanId, [FromBody] CreateSubscriptionPlanDto updateSubscriptionPlanDto)
        {
            var result = await _subscriptionPlanService.UpdateSubscriptionPlan(subscriptionPlanId, updateSubscriptionPlanDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteSubscriptionPlan/{subscriptionPlanId}")]
        public async Task<IActionResult> DeleteSubscriptionPlan(Guid subscriptionPlanId)
        {
            var result = await _subscriptionPlanService.DeleteSubscriptionPlan(subscriptionPlanId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetAllSubscriptionPlans")]
        public async Task<IActionResult> GetAllSubscriptionPlans()
        {
            var result = await _subscriptionPlanService.GetAllSubscriptionPlans();
            return Ok(result);
        }

        [HttpGet("GetSubscriptionPlanById/{subscriptionPlanId}")]
        public async Task<IActionResult> GetSubscriptionPlanById(Guid subscriptionPlanId)
        {
            var result = await _subscriptionPlanService.GetSubscriptionPlanById(subscriptionPlanId);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("GetSubscriptionPlansByGymId/{gymId}")]
        public async Task<IActionResult> GetSubscriptionPlansByGymId(Guid gymId)
        {
            var result = await _subscriptionPlanService.GetSubscriptionPlansByGymId(gymId);
            return Ok(result);
        }

    }
}
