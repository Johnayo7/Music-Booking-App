

using Music_Booking_App.Core.Helpers;
using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Services.Authentication.Configurations;
using Music_Booking_App.Services.Authentication.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Music_Booking_App.Services.Authentication.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IDapperQueryRepository<User> _appUserQueryRepository;
        private readonly IDapperCommandRepository<User> _appUserCommandRepository;
        private readonly IUserService _userService;
        private readonly JwtConfig _jwtOption;

        public TokenService(
            IOptions<JwtConfig> options,
            IDapperQueryRepository<User> appUserQueryRepository,
            IDapperCommandRepository<User> appUserCommandRepository,
            IUserService userService)
        {
            _jwtOption = options.Value;
            _appUserQueryRepository = appUserQueryRepository;
            _appUserCommandRepository = appUserCommandRepository;
            _userService = userService;
        }

        public async Task<(string Token, string RefreshToken)> GenerateTokenAsync(User user, bool isRememberMe = false)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email!),
                new (ClaimTypes.NameIdentifier, user.UserName!),
                new (ClaimTypes.SerialNumber, user.OrganizationId.ToString()!),
                new ("SecurityStamp", user.SecurityStamp)
            };

            // Conditionally add applicant_id and external_userId if they are not null
            claims.AddRange(new[]
            {
                string.IsNullOrWhiteSpace(user.ApplicantId) ? null: new Claim("applicant_id", user.ApplicantId),
                string.IsNullOrWhiteSpace(user.ExternalUserId) ? null: new Claim("external_userId", user.ExternalUserId)
            }.Where(c => c != null));

            var tokenLifetime = isRememberMe ? TimeSpan.FromDays(30).TotalSeconds : _jwtOption.TokenLifeTime;

            var token = ComputeToken(claims, (int)tokenLifetime);
            var refreshToken = await CreateAndUpdateRefreshTokenAsync(user);

            return (token, refreshToken);
        }

        public async Task<(string Token, string RefreshToken)?> ReGenerateTokenAsync(string token, string refreshToken)
        {
            var username = GetUsernameFromExpiredToken(token);
            if (username is null)
            {
                return null;
            }
            var query = new Dictionary<string, string>
            {
                { nameof(User.UserName), username }
            };

            var user = await _appUserQueryRepository.GetByDefaultAsync(query);

            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return (token, refreshToken);
        }

        public async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOption.SecretKey);

            var userClaims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Disable the default 5 minutes skew
            }, out var validatedToken);

            var securityStampFromToken = userClaims.FindFirst("SecurityStamp")?.Value;

            var userId = userClaims.FindFirst(ClaimTypes.Sid)?.Value;
            var user = await _userService.FindByIdAsync(userId); // Retrieve user by ID

            if (user == null || user.SecurityStamp != securityStampFromToken)
            {
                //throw new SecurityTokenException("Invalid token: SecurityStamp mismatch.");
                return null;
            }

            return userClaims;
        }

        public async Task<User> ValidateUserToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOption.SecretKey);

            var userClaims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Disable the default 5 minutes skew
            }, out var validatedToken);

            var securityStampFromToken = userClaims.FindFirst("SecurityStamp")?.Value;

            var userId = userClaims.FindFirst(ClaimTypes.Sid)?.Value;
            var user = await _userService.FindByIdAsync(userId); // Retrieve user by ID

            if (user == null || user.SecurityStamp != securityStampFromToken)
            {
                //throw new SecurityTokenException("Invalid token: SecurityStamp mismatch.");
                return null;
            }

            return user;
        }

        public async Task<ClaimsPrincipal> ValidateSecurityStamp(ClaimsPrincipal userClaims)
        {
            var securityStampFromToken = userClaims.FindFirst("SecurityStamp")?.Value;

            var userId = userClaims.FindFirst(ClaimTypes.Sid)?.Value;

            // Retrieve the user from the database using the ID
            var user = await _userService.FindByIdAsync(userId);

            if (user == null || user.SecurityStamp != securityStampFromToken)
            {
                return null;
            }

            return userClaims;
        }

        private async Task<string> CreateAndUpdateRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = randomNumber.ToBase64String();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtOption.RefreshTokenLiftTime);
            //user.SecurityStamp = Guid.NewGuid().ToString();

            await _appUserCommandRepository.UpdateAsync(user);
            return refreshToken;
        }
        private string ComputeToken(List<Claim> claims, int tokenLifetimeInSeconds)
        {
            var signInCredential = new SigningCredentials(
                    symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenLifetimeInSeconds),
                signingCredentials: signInCredential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private SymmetricSecurityKey symmetricSecurityKey => new(_jwtOption.SecretKey.GetBytes());
        public string? GetUsernameFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
