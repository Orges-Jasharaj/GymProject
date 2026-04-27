using GymProject.NutritionService.Services.Interface;
using GymProject.Shared.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.NutritionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }


        [HttpPost("CreateMeal")]
        public async Task<IActionResult> CreateMeal([FromBody] CreateMealDto createMealDto)
        {
            var result = await _mealService.CreateMealAsync(createMealDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteMeal/{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            var result = await _mealService.DeleteMealAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpPut("UpdateMeal/{id}")]
        public async Task<IActionResult> UpdateMeal(int id, [FromBody] CreateMealDto updateMealDto)
        {
            var result = await _mealService.UpdateMealAsync(id, updateMealDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetMealById/{id}")]
        public async Task<IActionResult> GetMealById(int id)
        {
            var result = await _mealService.GetMealByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("GetAllMeals")]
        public async Task<IActionResult> GetAllMeals()
        {
            var result = await _mealService.GetAllMealsAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
