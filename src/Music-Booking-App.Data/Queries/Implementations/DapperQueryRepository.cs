using Microsoft.Extensions.Configuration;
using Music_Booking_App.Data.Executers.Interfaces;
using Music_Booking_App.Data.Extensions;
using Music_Booking_App.Data.Helpers;
using Music_Booking_App.Data.Helpers.Interfaces;
using Music_Booking_App.Data.Queries.Interfaces;
using Music_Booking_App.Models.Enums;

namespace Music_Booking_App.Data.Queries.Implementations
{
    public class DapperQueryRepository<TEntity> : IDapperQueryRepository<TEntity> where TEntity : class
    {
        private readonly string? _connStr;
        private readonly IReadExecuter _executer;
        private readonly IReadUtilities _utilities;

        public DapperQueryRepository(IConfiguration configuration, IReadExecuter executer, IReadUtilities utilities)
        {
            _executer = executer;
            _utilities = utilities;
            _connStr = configuration.GetConnectionString("SecondaryDbConnectionString");
        }

        public async Task<TEntity?> FindByIdAsync(Guid id)
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var query = CustomQueries.GetById.Replace("#table", tableName);
            IEnumerable<TEntity?> entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, new { Id = id });
            return entities.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(int pageSize, int pageNumber)
        {
            var query = _utilities.GenerateSelectQuery<TEntity>(pageSize, pageNumber);
            var entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entities;
        }

        public IEnumerable<TEntity> GetBy(string query, DataSource dataSource)
        {
            var tableName = typeof(TEntity).GetReadTableName();
            query = query.Replace("#table", tableName);
            var entities = _executer.ExecuteReader<TEntity>(_utilities.GetConnectionString(dataSource), query, null);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetByAsync(Dictionary<string, string> criteria, int pageSize,
            int pageNumber)
        {
            var query = _utilities.GenerateSelectWhereQuery<TEntity>(criteria, pageNumber, pageSize);
            var entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetByAsyncForPartialMatch(Dictionary<string, string> criteria, int pageSize,
       int pageNumber)
        {
            var query = _utilities.GenerateSelectWhereQueryPartialMatch<TEntity>(criteria, pageNumber, pageSize);
            var entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entities;
        }

        public async Task<TEntity?> GetByDefaultAsync(Dictionary<string, string> criteria)
        {
            var query = _utilities.GenerateSelectSingleRecordQuery<TEntity>(criteria);
            IEnumerable<TEntity?> entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entities.FirstOrDefault();
        }

        public async Task<int> GetCountAsync()
        {
            var query = _utilities.GenerateSelectCountQuery<TEntity>();
            var response = await _executer.ExecuteReaderAsync<int>(_connStr, query, null);
            return response.FirstOrDefault();
        }

        public async Task<int> GetCountAsync(Dictionary<string, string> criteria)
        {
            var query = _utilities.GenerateSelectCountWhereQuery<TEntity>(criteria);
            var response = await _executer.ExecuteReaderAsync<int>(_connStr, query, null);
            return response.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> SearchMultipleValuesInSingleFieldAsync(
            KeyValuePair<string, List<string>> searchParams)
        {
            var query = _utilities.GenerateSelectInQuery<TEntity>(searchParams);
            var entities = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> SearchMultipleValuesInSingleFieldAsync(
            KeyValuePair<string, List<string>> searchParams, int pageSize, int pageNumber)
        {
            var query = _utilities.GenerateSelectInQuery<TEntity>(searchParams, pageSize, pageNumber);
            var entityObject = await _executer.ExecuteReaderAsync<TEntity>(_connStr, query, null);
            return entityObject;
        }

        public async Task<long> SearchMultipleValuesInSingleFieldCountAsync(KeyValuePair<string, List<string>> keyValuePair)
        {
            var query = _utilities.GenerateSelectCountInAsync<TEntity>(keyValuePair);
            var response = await _executer.ExecuteReaderAsync<long>(_connStr, query, null);
            return response.FirstOrDefault();
        }
    }
}
