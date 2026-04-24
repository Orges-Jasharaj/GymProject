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
                var response = await _httpClient.GetAsync($"/api/UserProfiles/GetProfileIdByUserId/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GymProject.Dtos.Responses.ResponseDto<Guid?>>();
                    if (result != null && result.Success)
                    {
                        return result.Data;
                    }
                }
                
                _logger.LogWarning("Failed to retrieve UserProfileId for UserId: {UserId} from IdentityService. Status: {StatusCode}", userId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call IdentityService to get UserProfileId for UserId: {UserId}", userId);
                return null;
            }
        }
    }
}
