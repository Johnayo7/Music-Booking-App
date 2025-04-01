

using Music_Booking_App.Data.Helpers;

namespace Music_Booking_App.Data.Commands.Interfaces
{
    public interface IRedisCommandRepository : IAutoDependencyRepository
    {
        void SetValue(string key, string value);
        Task SetValueAsync(string key, string value);
        Task SetValueWithExpiryAsync<T>(string key, T value, int expirationTimeInSeconds);
        Task<bool> RemoveKeyAsync(string key);
        bool RemoveKey(string key);
        long Increment(string key, string hashField);
        Task<long> IncrementAsync(string key, string hashField);
        long SetIncrementOffset(string key, string hashField, int offsetNumber);
        Task<string?> GetValueAsync(string key);
    }
}
