using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.SubscriptionService.Services.Interface
{
    public interface ISubscriptionPlanService
    {
        Task<ResponseDto<bool>> CreateSubscriptionPlan(CreateSubscriptionPlanDto createSubscriptionPlanDto);
        Task<ResponseDto<bool>> UpdateSubscriptionPlan(Guid id, CreateSubscriptionPlanDto updateSubscriptionPlanDto);
        Task<ResponseDto<bool>> DeleteSubscriptionPlan(Guid id);
        Task<ResponseDto<SubscriptionPlanDto>> GetSubscriptionPlanById(Guid id);
        Task<ResponseDto<List<SubscriptionPlanDto>>> GetSubscriptionPlansByGymId(Guid gymId);
        Task<ResponseDto<List<SubscriptionPlanDto>>> GetAllSubscriptionPlans();
    }
}
