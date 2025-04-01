

using Music_Booking_App.Data.Executers.Interfaces;
using Dapper;
using Npgsql;

namespace Music_Booking_App.Data.Executers.Implementations
{
    public class WriteExecuter : IWriteExecuter
    {
        public void ExecuteCommand<T>(string? connStr, string query, object param)
        {
            using var conn = new NpgsqlConnection(connStr);
            conn.Execute(query, param);
        }

        public async Task ExecuteCommandAsync<T>(string? connStr, string query, object param)
        {
            await using var conn = new NpgsqlConnection(connStr);
            await conn.ExecuteAsync(query, param);
        }

        public async Task ExecuteCommandAsync<T>(string query, object param, NpgsqlTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");

            await transaction.Connection.ExecuteAsync(query, param, transaction);
        }
    }
}
