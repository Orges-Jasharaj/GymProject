using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GymProject.Dtos.Responses;
using GymProject.Services.Interface;
using GymProject.Shared.Dtos.Responses;

namespace GymProject.Services.Implementation
{
    public class UserSubscriptionClient : IUserSubscriptionClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserSubscriptionClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSubscriptionClient(HttpClient httpClient, ILogger<UserSubscriptionClient> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<List<UserSubscriptionDto>>> GetUserSubscriptionsByUserId(string userId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/UserSubscription/GetUserSubscriptions/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("SubscriptionService returned {StatusCode} for user {UserId}", response.StatusCode, userId);
                    return ResponseDto<List<UserSubscriptionDto>>.Failure("Unable to retrieve subscription data.");
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseDto<List<UserSubscriptionDto>>>();
                return result ?? ResponseDto<List<UserSubscriptionDto>>.Failure("Invalid response from subscription service.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve subscriptions for user {UserId}", userId);
                return ResponseDto<List<UserSubscriptionDto>>.Failure("Failed to retrieve subscription data.");
            }
        }

        public async Task<ResponseDto<UserSubscriptionDto>> GetActiveUserSubscriptionByUserIdAsync(string userId)
        {
            try
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/UserSubscription/GetActiveSubscription/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("SubscriptionService returned {StatusCode} for active subscription of user {UserId}", response.StatusCode, userId);
                    return ResponseDto<UserSubscriptionDto>.Failure("Unable to retrieve active subscription data.");
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseDto<UserSubscriptionDto>>();
                return result ?? ResponseDto<UserSubscriptionDto>.Failure("Invalid response from subscription service.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve active subscription for user {UserId}", userId);
                return ResponseDto<UserSubscriptionDto>.Failure("Failed to retrieve active subscription data.");
            }
        }
    }
}
