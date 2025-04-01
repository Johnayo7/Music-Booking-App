using Music_Booking_App.Data.Helpers;

namespace Music_Booking_App.Data.Executers.Interfaces
{
    public interface IReadExecuter : IAutoDependencyRepository
    {
        IEnumerable<T> ExecuteReader<T>(string? connStr, string query, object? param);

        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string? connStr, string query, object? param);
    }
}
