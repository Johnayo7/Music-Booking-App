

using Music_Booking_App.Services.Authentication.Interfaces;
using System.Text;

namespace Music_Booking_App.Services.Authentication.Implementations
{
    public class PasswordHasher : IPasswordHash
    {

        public Task CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            return Task.CompletedTask;
        }
        public Task<bool> VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var ComputedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputedPassword.Length; i++)
                {
                    if (ComputedPassword[i] != passwordHash[i])

                        return Task.FromResult(false);
                }
            }
            return Task.FromResult(true);
        }
    }
}
