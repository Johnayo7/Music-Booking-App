

using Npgsql;

namespace Music_Booking_App.Data.Commands.Interfaces
{
    public interface IDapperCommandRepository<in TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task UpdateWithOpenDbTransactionAsync(TEntity entity, NpgsqlTransaction sqlTransaction);
        void Delete(Guid id);
        NpgsqlTransaction BeginTransaction();
        void CommitTransaction(NpgsqlTransaction sqlTransaction);
        void RollbackTransaction(NpgsqlTransaction sqlTransaction);
        Task AddWithOpenDbTransactionAsync(TEntity entity, NpgsqlTransaction sqlTransaction);
    }
}
