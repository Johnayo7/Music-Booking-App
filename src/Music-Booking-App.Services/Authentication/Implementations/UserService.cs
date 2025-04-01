

using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Entiites;
using Music_Booking_App.Services.Authentication.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Music_Booking_App.Services.Authentication.Implementations
{
    public class UserService : IUserService
    {
        private readonly IDapperCommandRepository<User> _appUserCommandRepository;
        private readonly IDapperQueryRepository<User> _appUserQueryRepository;
        private readonly IPasswordHash _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly int _maxFailedAttempts;
        private readonly int _lockoutDuration;

        public UserService(
            IDapperCommandRepository<User> userCommandRepository,
            IDapperQueryRepository<User> userQueryRepository,
            IPasswordHash passwordHasher,
            IConfiguration configuration)
        {
            _appUserCommandRepository = userCommandRepository;
            _appUserQueryRepository = userQueryRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _maxFailedAttempts = _configuration.GetValue<int>("LockOutSettings:MaxFailedAttempts", defaultValue: 4);
            _lockoutDuration = _configuration.GetValue<int>("LockOutSettings:LockOutDuration", defaultValue: 15);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var queryFilters = new Dictionary<string, string>
        {
           {nameof(User.Email), email}
        };
            var result = await _appUserQueryRepository.GetByDefaultAsync(queryFilters);

            return result;
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            var result = await _appUserQueryRepository.FindByIdAsync(Guid.Parse(userId));

            return result;
        }

        public async Task<bool> CreateAsync(User user)
        {
            await _appUserCommandRepository.AddAsync(user);
            return true;
        }

        public async Task<bool> AddPasswordAsync(User user, string password)
        {
            await _passwordHasher.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _appUserCommandRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            user.LastUpdateDate = DateTime.UtcNow;
            await _appUserCommandRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _passwordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
        }

        public async Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            if (!await CheckPasswordAsync(user, currentPassword))
                return false;

            await _passwordHasher.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            return await UpdateAsync(user);
        }

        public async Task<bool> IsLockedOutAsync(User user)
        {
            if (!user.LockoutEnabled)
                return false;

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                return true;

            // Reset lockout state if lockout period has ended
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.LockoutEnabled = false;
            await UpdateAsync(user);

            return false;
        }

        public async Task LockoutAsync(User user)
        {
            // lock out user after 5 failed attempts
            int maxFailedAccessAttempts = _maxFailedAttempts;
            TimeSpan lockoutDuration = TimeSpan.FromMinutes(_lockoutDuration);

            user.AccessFailedCount += 1;

            if (user.AccessFailedCount >= maxFailedAccessAttempts)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.UtcNow.Add(lockoutDuration);
                user.LockOutStart = DateTime.UtcNow;
                user.AccessFailedCount = 0; // Reset access failed count after locking out
            }

            await UpdateAsync(user);
        }
    }
}
