using Music_Booking_App.Models.Entiites;

namespace Music_Booking_App.Services.Authentication.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateAsync(User user);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByIdAsync(string userId);
        Task<bool> AddPasswordAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<bool> IsLockedOutAsync(User user);
        Task LockoutAsync(User user);
    }
}
