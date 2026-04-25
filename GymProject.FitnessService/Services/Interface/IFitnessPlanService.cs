using GymProject.Dtos.Requests;
using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.Services.Interface
{
    public interface IFitnessPlanService
    {
        Task<ResponseDto<bool>> CreateFitnessPlanAsync(CreateFitnessPlansDto fitnessPlanDto);
        Task<ResponseDto<FitnessPlansDto>> GetFitnessPlanByIdAsync(Guid id);
        Task<ResponseDto<List<FitnessPlansDto>>> GetAllFitnessPlansAsync();
        Task<ResponseDto<bool>> UpdateFitnessPlanAsync(Guid id, CreateFitnessPlansDto fitnessPlanDto);
        Task<ResponseDto<bool>> DeleteFitnessPlanAsync(Guid id);
        Task<ResponseDto<FitnessPlanDetailsDto>> GetFitnessPlanDetailsAsync(Guid id);
    }
}
