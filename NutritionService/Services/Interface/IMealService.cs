using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.NutritionService.Services.Interface
{
    public interface IMealService
    {
        Task<ResponseDto<bool>> CreateMealAsync(CreateMealDto createMealDto);
        Task<ResponseDto<MealDto>> GetMealByIdAsync(int id);
        Task<ResponseDto<List<MealDto>>> GetAllMealsAsync();
        Task<ResponseDto<bool>> UpdateMealAsync(int id, CreateMealDto updateMealDto);
        Task<ResponseDto<bool>> DeleteMealAsync(int id);
        Task<ResponseDto<NutritionPlanDto>> CreateNutritionPlanAsync(CreateNutritionPlanDto createNutritionPlanDto);
        Task<ResponseDto<List<NutritionPlanDto>>> GetAllNutritionPlansAsync();
        Task<ResponseDto<NutritionPlanDetailsDto>> GetNutritionPlanDetailsAsync(int id);
        Task<ResponseDto<bool>> AddMealToPlanAsync(CreatePlanMealDto createPlanMealDto);
        Task<ResponseDto<bool>> UpdateNutritionPlanAsync(int id, CreateNutritionPlanDto updateNutritionPlanDto);
        Task<ResponseDto<bool>> DeleteNutritionPlanAsync(int id);
        Task<ResponseDto<bool>> UpdateMealInPlanAsync(int planMealId, UpdatePlanMealDto updatePlanMealDto);
        Task<ResponseDto<bool>> DeleteMealFromPlanAsync(int planMealId);
    }
}
