

namespace Music_Booking_App.Services.Authentication.Interfaces
{
    public interface IPasswordHash
    {
        Task CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
