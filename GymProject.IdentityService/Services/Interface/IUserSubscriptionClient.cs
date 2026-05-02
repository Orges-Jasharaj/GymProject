using GymProject.Dtos.Responses;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.Services.Interface
{
    public interface IUserSubscriptionClient
    {
        Task<ResponseDto<List<UserSubscriptionDto>>> GetUserSubscriptionsByUserId(string userId);
        Task<ResponseDto<UserSubscriptionDto>> GetActiveUserSubscriptionByUserIdAsync(string userId);
    }
}
