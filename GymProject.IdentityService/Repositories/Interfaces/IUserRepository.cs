using GymProject.Dtos.Responses;

namespace GymProject.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllUsersAsync(bool includeInactive);
        Task<bool> ReactivateUserAsync(string userId);
    }
}
