
using Music_Booking_App.Data.Cache.Interfaces;
using StackExchange.Redis;

namespace Music_Booking_App.Data.Cache.Implementations
{
    public class RedisCacheStorage : IRedisCacheStorage
    {
        private readonly IRedisConnectionManager _connectionMultiplexer;

        public RedisCacheStorage(IRedisConnectionManager connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<IDatabase> GetDatabaseAsync()
        {
            return await _connectionMultiplexer.ConnectAsync();
        }
    }
}
