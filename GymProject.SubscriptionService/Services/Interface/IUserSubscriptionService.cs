using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Requests;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.SubscriptionService.Services.Interface
{
    public interface IUserSubscriptionService
    {
        Task<ResponseDto<bool>> CreateUserSubscription(CreateUserSubscriptionDto createUserSubscriptionDto);
        Task<ResponseDto<List<UserSubscriptionDto>>> GetUserSubscriptionsByUserId(string userId);
        Task<ResponseDto<UserSubscriptionDto>> GetActiveUserSubscription(string userId);
    }
}
