
namespace Music_Booking_App.Data.Helpers.Interfaces
{
    public interface IWriteUtilities : IAutoDependencyRepository
    {
        object GetObjectParams<TEntity>(TEntity entity) where TEntity : class;

        string GenerateInsertQuery<TEntity>(TEntity entity) where TEntity : class;

        string GenerateUpdateQuery<TEntity>(TEntity entity) where TEntity : class;
    }
}
