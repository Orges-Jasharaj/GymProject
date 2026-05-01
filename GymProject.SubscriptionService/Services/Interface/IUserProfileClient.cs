namespace GymProject.SubscriptionService.Services.Interface
{
    public interface IUserProfileClient
    {
        Task<Guid?> GetUserProfileIdByUserId(string userId);
    }
}
