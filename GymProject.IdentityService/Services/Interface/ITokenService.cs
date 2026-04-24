using GymProject.Dtos.Responses;
using GymProject.Models;
using System.Security.Claims;

namespace GymProject.Services.Interface
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, List<string> roles);
        RefreshTokenDto GenerateRrefreshToken();

        ClaimsPrincipal GetClaimsPrincipal(string token);
    }
}
