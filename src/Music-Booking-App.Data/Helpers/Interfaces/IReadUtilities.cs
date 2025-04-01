

using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Data.Helpers.Interfaces
{
    public interface IReadUtilities : IAutoDependencyRepository
    {
        string? GetConnectionString(DataSource dataSource);

        string GenerateSelectQuery<TEntity>(int pageSize, int pageNumber) where TEntity : class;

        string GenerateSelectSingleRecordQuery<TEntity>(Dictionary<string, string> criteria) where TEntity : class;

        string GenerateSelectWhereQuery<TEntity>(Dictionary<string, string> criteria, int pageNumber, int pageSize)
            where TEntity : class;

        string GenerateSelectInQuery<TEntity>(KeyValuePair<string, List<string>> criteria) where TEntity : class;

        string GenerateSelectInQuery<TEntity>(KeyValuePair<string, List<string>> criteria, int pageSize, int pageNumber)
            where TEntity : class;

        string GenerateSelectCountQuery<TEntity>() where TEntity : class;

        string GenerateSelectCountWhereQuery<TEntity>(Dictionary<string, string> criteria) where TEntity : class;

        string GenerateSelectCountInAsync<TEntity>(KeyValuePair<string, List<string>> criteria);
    }
}
