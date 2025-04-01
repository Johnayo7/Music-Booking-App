

using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Data.Queries.Interfaces
{
    public interface IDapperQueryRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int pageNumber);
        IEnumerable<TEntity> GetBy(string query, DataSource dataSource);
        Task<IEnumerable<TEntity>> GetByAsync(Dictionary<string, string> criteria, int pageSize, int pageNumber);
        Task<TEntity?> GetByDefaultAsync(Dictionary<string, string> criteria);
        Task<int> GetCountAsync(Dictionary<string, string> criteria);
        Task<int> GetCountAsync();
        Task<IEnumerable<TEntity>> SearchMultipleValuesInSingleFieldAsync(KeyValuePair<string, List<string>> searchParams);

        Task<IEnumerable<TEntity>> SearchMultipleValuesInSingleFieldAsync(KeyValuePair<string, List<string>> searchParams,
            int pageSize, int pageNumber);

        Task<long> SearchMultipleValuesInSingleFieldCountAsync(KeyValuePair<string, List<string>> keyValuePair);
    }
}
