using GymProject.Shared.Dtos.Requests;
using GymProject.SubscriptionService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymservice;

        public GymController(IGymService gymservice)
        {
            _gymservice = gymservice;
        }

        [HttpPost("CreateGym")]
        public async Task<IActionResult> CreateGym([FromBody] CreateGymDto createGymDto)
        {
            return Ok(await _gymservice.CreateGymAsync(createGymDto));
        }

        [HttpPut("UpdateGym/{gymId}")]
        public async Task<IActionResult> UpdateGym(Guid gymId, [FromBody] CreateGymDto updateGymDto)
        {
            return Ok(await _gymservice.UpdateGymAsync(gymId, updateGymDto));
        }

        [HttpDelete("DeleteGym/{gymId}")]
        public async Task<IActionResult> DeleteGym(Guid gymId)
        {
            return Ok(await _gymservice.DeleteGymAsync(gymId));
        }

        [HttpGet("GetAllGyms")]
        public async Task<IActionResult> GetAllGyms()
        {
            return Ok(await _gymservice.GetAllGyms());
        }

        [HttpGet("GetGymById/{gymId}")]
        public async Task<IActionResult> GetGymById(Guid gymId)
        {
            return Ok(await _gymservice.GetGymByIdAsync(gymId));
        }
    }
}
