using GymProject.Dtos.Requests;
using GymProject.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanExercisesController : ControllerBase
    {
        private readonly IPlanExercisesService _planExercisesService;

        public PlanExercisesController(IPlanExercisesService planExercisesService)
        {
            _planExercisesService = planExercisesService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlanExercisesDto dto)
        {
            var result = await _planExercisesService.AddPlanExerciseAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _planExercisesService.GetAllPlanExercisesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _planExercisesService.GetPlanExerciseByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreatePlanExercisesDto dto)
        {
            var result = await _planExercisesService.UpdatePlanExerciseAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _planExercisesService.DeletePlanExerciseAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}