namespace GymProject.Services.Interface
{
    public interface IUserProfileClient
    {
        Task<Guid?> GetUserProfileIdByUserId(string userId);
    }
}
