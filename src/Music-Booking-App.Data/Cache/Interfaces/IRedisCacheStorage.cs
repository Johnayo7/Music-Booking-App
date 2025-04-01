

using Music_Booking_App.Data.Helpers;
using StackExchange.Redis;

namespace Music_Booking_App.Data.Cache.Interfaces
{
    public interface IRedisCacheStorage : IAutoDependencyRepository
    {
        Task<IDatabase> GetDatabaseAsync();
    }
}
