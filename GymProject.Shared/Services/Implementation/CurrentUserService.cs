using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GymProject.Services.Implementation
{
    public class CurrentUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


    }
}
