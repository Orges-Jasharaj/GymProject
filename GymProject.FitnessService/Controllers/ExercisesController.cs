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
    public class ExercisesController : ControllerBase
    {
        private readonly IExercisesService _exercisesService;

        public ExercisesController(IExercisesService exercisesService)
        {
            _exercisesService = exercisesService;
        }


        [HttpPost("CreateExercise")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateExercises([FromBody] CreateExercisesDto createExercisesDto)
        {
            return Ok(await _exercisesService.CreateExercise(createExercisesDto));
        }

        [HttpDelete("DeleteExercise/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> DeleteExercises(Guid id)
        {
            return Ok(await _exercisesService.DeleteExercise(id));
        }

        [HttpGet("GetAllExercises")]
        [Authorize]
        public async Task<IActionResult> GetAllExercises()
        {
            return Ok(await _exercisesService.GetAllExercises());
        }

        [HttpGet("GetExerciseById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetExerciseById(Guid id)
        {
            return Ok(await _exercisesService.GetExerciseById(id));

        }

        [HttpGet("GetExercisesByMuscleGroup")]
        [Authorize]
        public async Task<IActionResult> GetExercisesByMuscleGroup([FromQuery] string muscleGroup)
        {
            return Ok(await _exercisesService.GetExercisesByMuscleGroup(muscleGroup));
        }

        [HttpPut("UpdateExercise/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateExercise(Guid id, [FromBody] CreateExercisesDto updateExercisesDto)
        {
            return Ok(await _exercisesService.UpdateExercise(id, updateExercisesDto));
        }
    }
}
