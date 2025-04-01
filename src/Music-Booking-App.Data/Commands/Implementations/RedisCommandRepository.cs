

using Music_Booking_App.Data.Cache.Interfaces;
using Music_Booking_App.Data.Commands.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Music_Booking_App.Data.Commands.Implementations
{
    public class RedisCommandRepository : IRedisCommandRepository
    {
        private readonly IRedisCacheStorage _cache;
        private readonly IDistributedCache _distributedCache;

        public RedisCommandRepository(IDistributedCache distributedCache, IRedisCacheStorage cache)
        {
            _distributedCache = distributedCache;
            _cache = cache;
        }

        public void SetValue(string key, string value)
        {
            _distributedCache.SetString(key, value);
        }

        public async Task SetValueAsync(string key, string value)
        {
            await _distributedCache.SetStringAsync(key, value);
        }

        public async Task SetValueWithExpiryAsync<T>(string key, T value, int expirationTimeInSeconds)
        {
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.UtcNow.AddSeconds(expirationTimeInSeconds)
            });
        }

        public async Task<bool> RemoveKeyAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
            return true;
        }

        public bool RemoveKey(string key)
        {
            _distributedCache.Remove(key);
            return true;
        }

        public long Increment(string key, string hashField)
        {
            return _cache.GetDatabaseAsync().Result.HashIncrement(key, hashField);
        }

        public async Task<long> IncrementAsync(string key, string hashField)
        {
            return await _cache.GetDatabaseAsync().Result.HashIncrementAsync(key, hashField);
        }

        public long SetIncrementOffset(string key, string hashField, int offsetNumber)
        {
            return _cache.GetDatabaseAsync().Result.HashIncrement(key, hashField, offsetNumber);
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }
    }
}
