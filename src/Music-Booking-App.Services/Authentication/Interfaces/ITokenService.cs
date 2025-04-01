

using Music_Booking_App.Models.Entiites;
using System.Security.Claims;

namespace Music_Booking_App.Services.Authentication.Interfaces
{
    public interface ITokenService
    {
        Task<(string Token, string RefreshToken)> GenerateTokenAsync(User user, bool isRememberMe = false);
        Task<(string Token, string RefreshToken)?> ReGenerateTokenAsync(string token, string refreshToken);
        string? GetUsernameFromExpiredToken(string token);
        Task<ClaimsPrincipal> ValidateToken(string token);
        Task<ClaimsPrincipal> ValidateSecurityStamp(ClaimsPrincipal principal);
        Task<User> ValidateUserToken(string token);
    }
}
