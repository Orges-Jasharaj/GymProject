using Dapper;
using GymProject.Data;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.NutritionService.Data;
using GymProject.NutritionService.Models;
using GymProject.NutritionService.Services.Interface;
using GymProject.Services.Implementation;
using GymProject.Services.Interface;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;
using Microsoft.EntityFrameworkCore;

namespace GymProject.NutritionService.Services.Implimentation
{
    public class MealService : IMealService
    {
        private readonly EntityContext _context;
        private readonly DapperContext _dapperContext;
        private readonly ILogger<MealService> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly CurrentUserService _currentUserService;

        public MealService(
            EntityContext context,
            DapperContext dapperContext,
            ILogger<MealService> logger,
            IAuditLogService auditLogService,
            CurrentUserService currentUserService)
        {
            _context = context;
            _dapperContext = dapperContext;
            _logger = logger;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }



        public async Task<ResponseDto<bool>> CreateMealAsync(CreateMealDto createMealDto)
        {
            var existingMeal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Name == createMealDto.Name);

            if (existingMeal != null)
                return ResponseDto<bool>.Failure("A meal with the same name already exists.");

            var currectUserId = _currentUserService.GetCurrentUserId();

            var meal = new Meals
            {
                Name = createMealDto.Name,
                Description = createMealDto.Description,
                Calories = createMealDto.Calories,
                Protein = createMealDto.Protein,
                Carbohydrates = createMealDto.Carbohydrates,
                Fats = createMealDto.Fats,
                CreatedBy = currectUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Meals.Add(meal); 

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _auditLogService.LogActivityAsync<Meals>(
                    currectUserId, "Create", "Meals", meal.Id.ToString(), null, meal);

                return ResponseDto<bool>.SuccessResponse(true, "Meal created successfully.");
            }

            return ResponseDto<bool>.Failure("Failed to create meal.");
        }

        public async Task<ResponseDto<bool>> DeleteMealAsync(int id)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return ResponseDto<bool>.Failure("Meal not found.");
            }

            _context.Meals.Remove(meal);
            if (await _context.SaveChangesAsync() > 0)
            {
                await _auditLogService.LogActivityAsync<Meals>(meal.CreatedBy, "Delete", "Meals", meal.Id.ToString(), null, meal);
                return ResponseDto<bool>.SuccessResponse(true, "Meal deleted successfully.");
            }

            return ResponseDto<bool>.Failure("Failed to delete meal.");
        }

        public async Task<ResponseDto<List<MealDto>>> GetAllMealsAsync()
        {
            //use dapper to get all meals
            try
            {
                var query = "SELECT * FROM Meals";
                using var connection = _dapperContext.CreateConnection();
                var meals = await connection.QueryAsync<MealDto>(query);
                return ResponseDto<List<MealDto>>.SuccessResponse(meals.ToList(), "Meals retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving meals.");
                return ResponseDto<List<MealDto>>.Failure("An error occurred while retrieving meals.");
            }
        }

        public async Task<ResponseDto<MealDto>> GetMealByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM Meals WHERE Id = @Id";
                using var connection = _dapperContext.CreateConnection();
                var meal = await connection.QueryFirstOrDefaultAsync<MealDto>(query, new { Id = id });
                if (meal == null)
                {
                    return ResponseDto<MealDto>.Failure("Meal not found.");
                }
                return ResponseDto<MealDto>.SuccessResponse(meal, "Meal retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the meal with ID: {MealId}", id);
                return ResponseDto<MealDto>.Failure("An error occurred while retrieving the meal.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateMealAsync(int id, CreateMealDto updateMealDto)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return ResponseDto<bool>.Failure("Meal not found.");
            }

            var currectUserId = _currentUserService.GetCurrentUserId();

            var oldMeal = new Meals
            {
                Id = meal.Id,
                Name = meal.Name,
                Description = meal.Description,
                Calories = meal.Calories,
                Protein = meal.Protein,
                Carbohydrates = meal.Carbohydrates,
                Fats = meal.Fats,
                CreatedBy = meal.CreatedBy,
                CreatedAt = meal.CreatedAt,
                UpdatedBy = meal.UpdatedBy,
                UpdatedAt = meal.UpdatedAt
            };

            meal.Name = updateMealDto.Name;
            meal.Description = updateMealDto.Description;
            meal.Calories = updateMealDto.Calories;
            meal.Protein = updateMealDto.Protein;
            meal.Carbohydrates = updateMealDto.Carbohydrates;
            meal.Fats = updateMealDto.Fats;
            meal.UpdatedBy = currectUserId;
            meal.UpdatedAt = DateTime.UtcNow;

            if (await _context.SaveChangesAsync() > 0)
            {
                await _auditLogService.LogActivityAsync<Meals>(currectUserId, "Update", "Meals", meal.Id.ToString(), oldMeal, meal);
                return ResponseDto<bool>.SuccessResponse(true, "Meal updated successfully.");
            }

            return ResponseDto<bool>.Failure("Failed to update meal.");
        }
    }
}
