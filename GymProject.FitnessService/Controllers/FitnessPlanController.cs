using GymProject.Dtos.Requests;
using GymProject.Models;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetFitnessPlanById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetFitnessPlanById(Guid id)
        {
            var result = await _fitnessPlanService.GetFitnessPlanByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateFitnessPlan")]
        [Authorize]
        public async Task<IActionResult> CreateFitnessPlan([FromBody] CreateFitnessPlansDto createFitnessPlansDto)
        {
            var result = await _fitnessPlanService.CreateFitnessPlanAsync(createFitnessPlansDto);
            return Ok(result);
        }

        [HttpPut("UpdateFitnessPlan/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFitnessPlan(Guid id, [FromBody] CreateFitnessPlansDto updateFitnessPlansDto)
        {
            var result = await _fitnessPlanService.UpdateFitnessPlanAsync(id, updateFitnessPlansDto);
            return Ok(result);
        }

        [HttpDelete("DeleteFitnessPlan/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFitnessPlan(Guid id)
        {
            var result = await _fitnessPlanService.DeleteFitnessPlanAsync(id);
            return Ok(result);
        }

        [HttpGet("GetAllFitnessPlans")]
        [Authorize]
        public async Task<IActionResult> GetAllFitnessPlans()
        {
            var result = await _fitnessPlanService.GetAllFitnessPlansAsync();
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var result = await _fitnessPlanService.GetFitnessPlanDetailsAsync(id);
            return Ok(result);
        }
    }
}
