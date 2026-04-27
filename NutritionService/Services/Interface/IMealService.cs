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
    }
}
