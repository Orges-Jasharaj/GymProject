using GymProject.NutritionService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using GymProject.Shared.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymProject.NutritionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost("CreateNutritionPlan")]
        public async Task<IActionResult> CreateNutritionPlan([FromBody] CreateNutritionPlanDto createNutritionPlanDto)
        {
            var result = await _mealService.CreateNutritionPlanAsync(createNutritionPlanDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("GetAllNutritionPlans")]
        public async Task<IActionResult> GetAllNutritionPlans()
        {
            var result = await _mealService.GetAllNutritionPlansAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet("GetNutritionPlanDetails/{id}")]
        public async Task<IActionResult> GetNutritionPlanDetails(int id)
        {
            var result = await _mealService.GetNutritionPlanDetailsAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost("AddMealToNutritionPlan")]
        public async Task<IActionResult> AddMealToNutritionPlan([FromBody] CreatePlanMealDto createPlanMealDto)
        {
            var result = await _mealService.AddMealToPlanAsync(createPlanMealDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("UpdateNutritionPlan/{id}")]
        public async Task<IActionResult> UpdateNutritionPlan(int id, [FromBody] CreateNutritionPlanDto updateNutritionPlanDto)
        {
            var result = await _mealService.UpdateNutritionPlanAsync(id, updateNutritionPlanDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeleteNutritionPlan/{id}")]
        public async Task<IActionResult> DeleteNutritionPlan(int id)
        {
            var result = await _mealService.DeleteNutritionPlanAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("UpdatePlanMeal/{planMealId}")]
        public async Task<IActionResult> UpdatePlanMeal(int planMealId, [FromBody] UpdatePlanMealDto updatePlanMealDto)
        {
            var result = await _mealService.UpdateMealInPlanAsync(planMealId, updatePlanMealDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("DeletePlanMeal/{planMealId}")]
        public async Task<IActionResult> DeletePlanMeal(int planMealId)
        {
            var result = await _mealService.DeleteMealFromPlanAsync(planMealId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
