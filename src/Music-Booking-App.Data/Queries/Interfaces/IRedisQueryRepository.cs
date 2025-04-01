

using Music_Booking_App.Data.Helpers;

namespace Music_Booking_App.Data.Queries.Interfaces
{
    public interface IRedisQueryRepository : IAutoDependencyRepository
    {
        Task<string?> GetValueAsync(string key);
    }
}
