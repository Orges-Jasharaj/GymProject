using GymProject.Models;

namespace GymProject.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<int> CreateAsync(UserProfile userProfile);
        Task<int> UpdateAsync(UserProfile userProfile);
        Task<int> DeleteAsync(Guid id);
    }
}
