using Dapper;
using GymProject.Data;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.NutritionService.Data;
using GymProject.NutritionService.Enums;
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

        public async Task<ResponseDto<NutritionPlanDto>> CreateNutritionPlanAsync(CreateNutritionPlanDto createNutritionPlanDto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var existingPlan = await _context.NutritionPlans
                    .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.Name == createNutritionPlanDto.Name);

                if (existingPlan != null)
                    return ResponseDto<NutritionPlanDto>.Failure("A nutrition plan with this name already exists.");

                var nutritionPlan = new NutritionPlan
                {
                    Name = createNutritionPlanDto.Name,
                    UserId = currentUserId,
                    Goal = GoalType.Maintain,
                    TotalCalories = 0,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.NutritionPlans.Add(nutritionPlan);
                var saved = await _context.SaveChangesAsync();

                if (saved <= 0)
                    return ResponseDto<NutritionPlanDto>.Failure("Failed to create nutrition plan.");

                var result = new NutritionPlanDto
                {
                    Id = nutritionPlan.Id,
                    Name = nutritionPlan.Name
                };

                return ResponseDto<NutritionPlanDto>.SuccessResponse(result, "Nutrition plan created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating nutrition plan");
                return ResponseDto<NutritionPlanDto>.Failure("Failed to create nutrition plan.");
            }
        }

        public async Task<ResponseDto<List<NutritionPlanDto>>> GetAllNutritionPlansAsync()
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var plans = await _context.NutritionPlans
                    .Where(x => x.UserId == currentUserId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new NutritionPlanDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync();

                return ResponseDto<List<NutritionPlanDto>>.SuccessResponse(plans, "Nutrition plans retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nutrition plans");
                return ResponseDto<List<NutritionPlanDto>>.Failure("Failed to get nutrition plans.");
            }
        }

        public async Task<ResponseDto<NutritionPlanDetailsDto>> GetNutritionPlanDetailsAsync(int id)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var nutritionPlan = await _context.NutritionPlans
                    .Include(x => x.PlanMeals)
                    .ThenInclude(x => x.Meal)
                    .FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId);

                if (nutritionPlan == null)
                    return ResponseDto<NutritionPlanDetailsDto>.Failure("Nutrition plan not found.");

                var result = new NutritionPlanDetailsDto
                {
                    Id = nutritionPlan.Id,
                    Name = nutritionPlan.Name,
                    Meals = nutritionPlan.PlanMeals
                        .OrderBy(x => GetDayOrder(x.DayOfWeek))
                        .ThenBy(x => x.MealType)
                        .Select(x => new MealInPlanDto
                        {
                            PlanMealId = x.Id,
                            MealId = x.MealId,
                            DayOfWeek = x.DayOfWeek,
                            MealType = x.MealType.ToString(),
                            Name = x.Meal.Name,
                            Description = x.Meal.Description,
                            Calories = x.Meal.Calories,
                            Protein = x.Meal.Protein,
                            Carbohydrates = x.Meal.Carbohydrates,
                            Fats = x.Meal.Fats
                        })
                        .ToList()
                };

                return ResponseDto<NutritionPlanDetailsDto>.SuccessResponse(result, "Nutrition plan details retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nutrition plan details with Id {Id}", id);
                return ResponseDto<NutritionPlanDetailsDto>.Failure("Failed to get nutrition plan details.");
            }
        }

        public async Task<ResponseDto<bool>> AddMealToPlanAsync(CreatePlanMealDto createPlanMealDto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var nutritionPlan = await _context.NutritionPlans
                    .FirstOrDefaultAsync(x => x.Id == createPlanMealDto.NutritionPlanId && x.UserId == currentUserId);

                if (nutritionPlan == null)
                    return ResponseDto<bool>.Failure("Nutrition plan not found.");

                if (!Enum.TryParse<MealType>(createPlanMealDto.MealType, true, out var mealType))
                    return ResponseDto<bool>.Failure("Invalid meal type.");

                var meal = new Meals
                {
                    Name = createPlanMealDto.Name,
                    Description = createPlanMealDto.Description,
                    Calories = createPlanMealDto.Calories,
                    Protein = createPlanMealDto.Protein,
                    Carbohydrates = createPlanMealDto.Carbohydrates,
                    Fats = createPlanMealDto.Fats,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Meals.Add(meal);
                await _context.SaveChangesAsync();

                var planMeal = new PlanMeals
                {
                    NutritionPlanId = createPlanMealDto.NutritionPlanId,
                    MealId = meal.Id,
                    DayOfWeek = createPlanMealDto.DayOfWeek,
                    MealType = mealType,
                    TimeOfDay = MapMealTypeToTime(mealType),
                    PortionSize = 1
                };

                _context.PlanMeals.Add(planMeal);
                var saved = await _context.SaveChangesAsync();

                if (saved <= 0)
                    return ResponseDto<bool>.Failure("Failed to add meal to plan.");

                return ResponseDto<bool>.SuccessResponse(true, "Meal added to nutrition plan successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding meal to nutrition plan");
                return ResponseDto<bool>.Failure("Failed to add meal to nutrition plan.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateNutritionPlanAsync(int id, CreateNutritionPlanDto updateNutritionPlanDto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var nutritionPlan = await _context.NutritionPlans
                    .FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId);

                if (nutritionPlan == null)
                    return ResponseDto<bool>.Failure("Nutrition plan not found.");

                nutritionPlan.Name = updateNutritionPlanDto.Name;
                nutritionPlan.UpdatedBy = currentUserId;
                nutritionPlan.UpdatedAt = DateTime.UtcNow;

                var saved = await _context.SaveChangesAsync();
                if (saved <= 0)
                    return ResponseDto<bool>.Failure("Failed to update nutrition plan.");

                return ResponseDto<bool>.SuccessResponse(true, "Nutrition plan updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating nutrition plan with Id {Id}", id);
                return ResponseDto<bool>.Failure("Failed to update nutrition plan.");
            }
        }

        public async Task<ResponseDto<bool>> DeleteNutritionPlanAsync(int id)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var nutritionPlan = await _context.NutritionPlans
                    .Include(x => x.PlanMeals)
                    .FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId);

                if (nutritionPlan == null)
                    return ResponseDto<bool>.Failure("Nutrition plan not found.");

                var planMealIds = nutritionPlan.PlanMeals.Select(x => x.Id).ToList();
                var mealIds = nutritionPlan.PlanMeals.Select(x => x.MealId).Distinct().ToList();

                if (planMealIds.Count > 0)
                {
                    var planMeals = await _context.PlanMeals.Where(x => planMealIds.Contains(x.Id)).ToListAsync();
                    _context.PlanMeals.RemoveRange(planMeals);
                }

                if (mealIds.Count > 0)
                {
                    var orphanMealIds = new List<int>();
                    foreach (var mealId in mealIds)
                    {
                        var hasOtherReferences = await _context.PlanMeals.AnyAsync(x => x.MealId == mealId && !planMealIds.Contains(x.Id));
                        if (!hasOtherReferences)
                        {
                            orphanMealIds.Add(mealId);
                        }
                    }

                    if (orphanMealIds.Count > 0)
                    {
                        var meals = await _context.Meals.Where(x => orphanMealIds.Contains(x.Id)).ToListAsync();
                        _context.Meals.RemoveRange(meals);
                    }
                }

                _context.NutritionPlans.Remove(nutritionPlan);
                var saved = await _context.SaveChangesAsync();

                if (saved <= 0)
                    return ResponseDto<bool>.Failure("Failed to delete nutrition plan.");

                return ResponseDto<bool>.SuccessResponse(true, "Nutrition plan deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting nutrition plan with Id {Id}", id);
                return ResponseDto<bool>.Failure("Failed to delete nutrition plan.");
            }
        }

        public async Task<ResponseDto<bool>> UpdateMealInPlanAsync(int planMealId, UpdatePlanMealDto updatePlanMealDto)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var planMeal = await _context.PlanMeals
                    .Include(x => x.NutritionPlan)
                    .Include(x => x.Meal)
                    .FirstOrDefaultAsync(x => x.Id == planMealId);

                if (planMeal == null || planMeal.NutritionPlan.UserId != currentUserId)
                    return ResponseDto<bool>.Failure("Plan meal not found.");

                if (!Enum.TryParse<MealType>(updatePlanMealDto.MealType, true, out var mealType))
                    return ResponseDto<bool>.Failure("Invalid meal type.");

                planMeal.DayOfWeek = updatePlanMealDto.DayOfWeek;
                planMeal.MealType = mealType;
                planMeal.TimeOfDay = MapMealTypeToTime(mealType);

                planMeal.Meal.Name = updatePlanMealDto.Name;
                planMeal.Meal.Description = updatePlanMealDto.Description;
                planMeal.Meal.Calories = updatePlanMealDto.Calories;
                planMeal.Meal.Protein = updatePlanMealDto.Protein;
                planMeal.Meal.Carbohydrates = updatePlanMealDto.Carbohydrates;
                planMeal.Meal.Fats = updatePlanMealDto.Fats;
                planMeal.Meal.UpdatedBy = currentUserId;
                planMeal.Meal.UpdatedAt = DateTime.UtcNow;

                var saved = await _context.SaveChangesAsync();
                if (saved <= 0)
                    return ResponseDto<bool>.Failure("Failed to update meal in plan.");

                return ResponseDto<bool>.SuccessResponse(true, "Meal in plan updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan meal with Id {PlanMealId}", planMealId);
                return ResponseDto<bool>.Failure("Failed to update meal in plan.");
            }
        }

        public async Task<ResponseDto<bool>> DeleteMealFromPlanAsync(int planMealId)
        {
            try
            {
                var currentUserId = _currentUserService.GetCurrentUserId();
                var planMeal = await _context.PlanMeals
                    .Include(x => x.NutritionPlan)
                    .FirstOrDefaultAsync(x => x.Id == planMealId);

                if (planMeal == null || planMeal.NutritionPlan.UserId != currentUserId)
                    return ResponseDto<bool>.Failure("Plan meal not found.");

                var mealId = planMeal.MealId;
                _context.PlanMeals.Remove(planMeal);
                await _context.SaveChangesAsync();

                var hasOtherReferences = await _context.PlanMeals.AnyAsync(x => x.MealId == mealId);
                if (!hasOtherReferences)
                {
                    var meal = await _context.Meals.FindAsync(mealId);
                    if (meal != null)
                    {
                        _context.Meals.Remove(meal);
                        await _context.SaveChangesAsync();
                    }
                }

                return ResponseDto<bool>.SuccessResponse(true, "Meal removed from plan successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan meal with Id {PlanMealId}", planMealId);
                return ResponseDto<bool>.Failure("Failed to delete meal from plan.");
            }
        }

        private static int GetDayOrder(string dayOfWeek)
        {
            return dayOfWeek?.ToLowerInvariant() switch
            {
                "monday" => 1,
                "tuesday" => 2,
                "wednesday" => 3,
                "thursday" => 4,
                "friday" => 5,
                "saturday" => 6,
                "sunday" => 7,
                _ => 8
            };
        }

        private static TimeOnly MapMealTypeToTime(MealType mealType)
        {
            return mealType switch
            {
                MealType.Breakfast => new TimeOnly(8, 0),
                MealType.Lunch => new TimeOnly(13, 0),
                MealType.Snack => new TimeOnly(16, 0),
                MealType.Dinner => new TimeOnly(20, 0),
                _ => new TimeOnly(12, 0)
            };
        }
    }
}
