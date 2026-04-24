using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;

namespace GymProject.Services.Interface
{
    public interface IPlanExercisesService
    {
        Task<ResponseDto<bool>> AddPlanExerciseAsync(CreatePlanExercisesDto planExercise);
        Task<ResponseDto<bool>> DeletePlanExerciseAsync(Guid id);
        Task<ResponseDto<List<PlanExercisesDto>>> GetAllPlanExercisesAsync();
        Task<ResponseDto<PlanExercisesDto>> GetPlanExerciseByIdAsync(Guid id);
        Task<ResponseDto<bool>> UpdatePlanExerciseAsync(Guid id, CreatePlanExercisesDto planExercise);
    }
}
