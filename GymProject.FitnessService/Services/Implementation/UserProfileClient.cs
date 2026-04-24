using GymProject.Services.Interface;

namespace GymProject.Services.Implementation
{
    public class UserProfileClient : IUserProfileClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProfileClient> _logger;

        public UserProfileClient(HttpClient httpClient, ILogger<UserProfileClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Guid?> GetUserProfileIdByUserId(string userId)
        {
            try
            {
                // In a real microservice, this would call the IdentityService API.
                // For now, we will assume the UserId string is actually a Guid, or we just return a dummy Guid if it's not.
                if (Guid.TryParse(userId, out var guid))
                {
                    return guid;
                }
                
                // Return a dummy value to allow testing until the API integration is built
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get UserProfileId for UserId: {UserId}", userId);
                return null;
            }
        }
    }
}
