

using Music_Booking_App.Data.Commands.Interfaces;
using Music_Booking_App.Data.Executers.Interfaces;
using Music_Booking_App.Data.Extensions;
using Music_Booking_App.Data.Helpers;
using Music_Booking_App.Data.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Music_Booking_App.Data.Commands.Implementations
{
    public class DapperCommandRepository<TEntity> : IDapperCommandRepository<TEntity> where TEntity : class
    {
        private readonly string? _connectionString;
        private readonly IWriteUtilities _utilities;
        private readonly IWriteExecuter _writeExecuter;

        public DapperCommandRepository(IConfiguration configuration, IWriteExecuter writeExecuter,
            IWriteUtilities utilities)
        {
            _writeExecuter = writeExecuter;
            _utilities = utilities;
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }

        public void Add(TEntity entity)
        {
            var query = _utilities.GenerateInsertQuery(entity);
            _writeExecuter
                .ExecuteCommand<TEntity>(_connectionString, query, _utilities.GetObjectParams(entity));
        }

        public async Task AddAsync(TEntity entity)
        {
            var query = _utilities.GenerateInsertQuery(entity);
            await _writeExecuter
                .ExecuteCommandAsync<TEntity>(_connectionString, query, _utilities.GetObjectParams(entity));
        }

        public async Task AddWithOpenDbTransactionAsync(TEntity entity, NpgsqlTransaction sqlTransaction)
        {
            var query = _utilities.GenerateInsertQuery(entity);
            await _writeExecuter
                .ExecuteCommandAsync<TEntity>(query, _utilities.GetObjectParams(entity), sqlTransaction);
        }

        public void Update(TEntity entity)
        {
            var query = _utilities.GenerateUpdateQuery(entity);
            _writeExecuter
                .ExecuteCommand<TEntity>(_connectionString, query, _utilities.GetObjectParams(entity));
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var query = _utilities.GenerateUpdateQuery(entity);
            await _writeExecuter
                .ExecuteCommandAsync<TEntity>(_connectionString, query, _utilities.GetObjectParams(entity));
        }

        public async Task UpdateWithOpenDbTransactionAsync(TEntity entity, NpgsqlTransaction sqlTransaction)
        {
            var query = _utilities.GenerateUpdateQuery(entity);
            await _writeExecuter
                .ExecuteCommandAsync<TEntity>(query, _utilities.GetObjectParams(entity), sqlTransaction);
        }

        public void Delete(Guid id)
        {
            var tableName = typeof(TEntity).GetWriteTableName();
            var query = CustomQueries.DeleteById.Replace("#table", tableName);
            _writeExecuter.ExecuteCommand<TEntity>(_connectionString, query, new { ID = id });
        }

        public NpgsqlTransaction BeginTransaction()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            return sqlTransaction;
        }

        public void CommitTransaction(NpgsqlTransaction sqlTransaction)
        {
            try
            {
                if (sqlTransaction.Connection != null)
                {
                    using var conn = sqlTransaction.Connection;
                    sqlTransaction.Commit();
                    conn.Close();
                }
            }
            finally
            {
                if (sqlTransaction.Connection != null && sqlTransaction.Connection.State == ConnectionState.Open)
                    sqlTransaction.Connection.Close();
            }
        }

        public void RollbackTransaction(NpgsqlTransaction sqlTransaction)
        {
            try
            {
                if (sqlTransaction.Connection != null)
                {
                    using var conn = sqlTransaction.Connection;
                    sqlTransaction.Rollback();
                    conn.Close();
                }
            }
            finally
            {
                if (sqlTransaction.Connection != null && sqlTransaction.Connection.State == ConnectionState.Open)
                    sqlTransaction.Connection.Close();
            }
        }
    }
}
