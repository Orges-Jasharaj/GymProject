using GymProject.Dtos.Requests;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FitnessPlanController : ControllerBase
    {
        private readonly IFitnessPlanService _fitnessPlanService;

        public FitnessPlanController(IFitnessPlanService fitnessPlanService)
        {
            _fitnessPlanService = fitnessPlanService;
        }

        [HttpGet("GetFitnessPlanByUserId")]
        public async Task<IActionResult> GetFitnessPlanByUserId(Guid userId)
        {
            var result = await _fitnessPlanService.GetFitnessPlanByIdAsync(userId);
            return Ok(result);
        }

        [HttpPost("CreateFitnessPlan")]
        public async Task<IActionResult> CreateFitnessPlan(CreateFitnessPlansDto createFitnessPlansDto)
        {
            var result = await _fitnessPlanService.CreateFitnessPlanAsync(createFitnessPlansDto);
            return Ok(result);
        }

        [HttpPut("UpdateFitnessPlan")]
        public async Task<IActionResult> UpdateFitnessPlan(Guid id,CreateFitnessPlansDto updateFitnessPlansDto)
        {
            var result = await _fitnessPlanService.UpdateFitnessPlanAsync(id,updateFitnessPlansDto);
            return Ok(result);
        }

        [HttpDelete("DeleteFitnessPlan")]
        public async Task<IActionResult> DeleteFitnessPlan(Guid id)
        {
            var result = await _fitnessPlanService.DeleteFitnessPlanAsync(id);
            return Ok(result);
        }

        [HttpGet("GetAllFitnessPlans")]
        public async Task<IActionResult> GetAllFitnessPlans()
        {
            var result = await _fitnessPlanService.GetAllFitnessPlansAsync();
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var result = await _fitnessPlanService.GetFitnessPlanDetailsAsync(id);
            return Ok(result);
        }
    }
}
