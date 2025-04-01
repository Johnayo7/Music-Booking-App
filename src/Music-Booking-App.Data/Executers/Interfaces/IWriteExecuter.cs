

using Music_Booking_App.Data.Helpers;
using Npgsql;

namespace Music_Booking_App.Data.Executers.Interfaces
{
    public interface IWriteExecuter : IAutoDependencyRepository
    {
        void ExecuteCommand<T>(string? connStr, string query, object param);

        Task ExecuteCommandAsync<T>(string? connStr, string query, object param);

        Task ExecuteCommandAsync<T>(string query, object param, NpgsqlTransaction transaction);
    }
}
