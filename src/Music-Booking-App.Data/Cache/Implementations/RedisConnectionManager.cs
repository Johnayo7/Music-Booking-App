

using Music_Booking_App.Data.Cache.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Music_Booking_App.Data.Cache.Implementations
{
    public class RedisConnectionManager : IRedisConnectionManager, IDisposable
    {
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly string? _redisUrl;
        private IDatabase? _cache;
        private volatile ConnectionMultiplexer? _muxer;

        public RedisConnectionManager(IConfiguration configuration)
        {
            _redisUrl = configuration.GetSection("RedisConfig")["Url"];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<IDatabase> ConnectAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            if (_cache != null) return _cache;
            await _connectionLock.WaitAsync(token);
            try
            {
                if (_cache == null)
                {
                    _muxer = await ConnectionMultiplexer.ConnectAsync(_redisUrl);
                    _cache = _muxer.GetDatabase();
                }
            }
            finally
            {
                _connectionLock.Release();
            }

            return _cache;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_muxer != null) _muxer.Close();
        }
    }
}
