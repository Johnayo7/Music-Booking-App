
using Music_Booking_App.Data.Queries.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Music_Booking_App.Data.Queries.Implementations
{
    public class RedisQueryRepository : IRedisQueryRepository
    {
        private readonly IDistributedCache _distributedCache;

        public RedisQueryRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }

        public string? GetValue(string key)
        {
            return _distributedCache.GetString(key);
        }
    }
}
