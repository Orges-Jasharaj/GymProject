using GymProject.SubscriptionService.Services.Interface;
using System.Net.Http.Headers;

namespace GymProject.SubscriptionService.Services.Implementation
{
    public class UserProfileClient : IUserProfileClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProfileClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProfileClient(HttpClient httpClient, ILogger<UserProfileClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid?> GetUserProfileIdByUserId(string userId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

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
