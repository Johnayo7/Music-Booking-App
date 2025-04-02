

using Microsoft.Extensions.Configuration;
using Music_Booking_App.Data.Extensions;
using Music_Booking_App.Data.Helpers.Interfaces;
using Music_Booking_App.Models.Enums;
using System.Text;

namespace Music_Booking_App.Data.Helpers.Implementations
{
    public class ReadUtilities : IReadUtilities
    {
        private readonly string? _connStr;
        private readonly string? _secConnStr;

        public ReadUtilities(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DbConnectionString");
            _secConnStr = configuration.GetConnectionString("SecondaryDbConnectionString");
        }

        public string? GetConnectionString(DataSource dataSource)
        {
            return dataSource switch
            {
                DataSource.PrimaryDatabase => _connStr,
                DataSource.SecondaryDatabase => _secConnStr,
                _ => string.Empty
            };
        }

        public string GenerateSelectQuery<TEntity>(int pageSize, int pageNumber) where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var query =
                $"SELECT * FROM {tableName} WHERE \"IsDeleted\" is false ORDER BY \"CreationDate\" DESC LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";
            return query;
        }

        public string GenerateSelectSingleRecordQuery<TEntity>(Dictionary<string, string> criteria) where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            criteria.TryAdd("IsDeleted", "false");
            var selectQuery = new StringBuilder($"SELECT * FROM {tableName} WHERE ");
            var count = 1;
            foreach (var item in criteria)
            {
                selectQuery.Append($"\"{item.Key}\" = '{item.Value}' ");
                if (criteria.Count > count) selectQuery.Append("AND ");
                count++;
            }

            return $"{selectQuery} ORDER BY \"CreationDate\" LIMIT 1";
        }

        public string GenerateSelectWhereQuery<TEntity>(Dictionary<string, string> criteria, int pageNumber, int pageSize)
            where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            criteria.TryAdd("IsDeleted", "false");
            var selectQuery = new StringBuilder($"SELECT * FROM {tableName} WHERE ");
            var count = 1;
            foreach (var item in criteria)
            {
                var columnName = char.ToUpper(item.Key[0]) + item.Key.Substring(1);
                selectQuery.Append($"\"{columnName}\" = '{item.Value.Trim()}' ");
                if (criteria.Count > count) selectQuery.Append("AND ");
                count++;
            }

            return $"{selectQuery} ORDER BY \"CreationDate\" DESC LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";
        }

        public string GenerateSelectWhereQueryPartialMatch<TEntity>(Dictionary<string, string> criteria, int pageNumber, int pageSize)
    where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            criteria.TryAdd("IsDeleted", "false"); // Ensure non-deleted records
            var selectQuery = new StringBuilder($"SELECT * FROM {tableName} WHERE ");

            string[] exactMatchColumns = { "IsDeleted", "Status", "AccountStatus", "EventStatus", "ArtisteId", "EventOrganizerId" };
            string[] partialMatchWithCasting = { "Name", "Genre", "Location", "OrganizerName", "EventName", "ArtisteName" };

            var conditions = new List<string>();
            var partialMatchConditions = new List<string>();

            foreach (var item in criteria)
            {
                var columnName = char.ToUpper(item.Key[0]) + item.Key.Substring(1);
                if (exactMatchColumns.Contains(columnName))
                {
                    conditions.Add($"\"{columnName}\" = '{item.Value.Trim()}' ");
                }
                else if (partialMatchWithCasting.Contains(columnName))
                {
                    partialMatchConditions.Add($"CAST(\"{columnName}\" AS TEXT) ILIKE '%{item.Value.Trim()}%' ");
                }
                else
                {
                    partialMatchConditions.Add($"\"{columnName}\" ILIKE '%{item.Value.Trim()}%' ");
                }
            }

            if (partialMatchConditions.Any())
            {
                selectQuery.Append("(").Append(string.Join(" OR ", partialMatchConditions)).Append(")");
            }
            else
            {
                // If no partial match criteria is found, force query to return zero results
                return "SELECT * FROM " + tableName + " WHERE 1 = 0";
            }

            // Add "IsDeleted" condition after ensuring valid partial match conditions exist
            var isDeletedCondition = conditions.FirstOrDefault(c => c.Contains("\"IsDeleted\""));
            if (!string.IsNullOrEmpty(isDeletedCondition))
            {
                selectQuery.Append(" AND ").Append(isDeletedCondition);
            }

            return $"{selectQuery} ORDER BY \"CreationDate\" DESC LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";
        }

        public string GenerateSelectInQuery<TEntity>(KeyValuePair<string, List<string>> criteria) where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var selectQuery = new StringBuilder($"SELECT * FROM {tableName} WHERE \"{criteria.Key}\" IN (");

            foreach (var itemValue in criteria.Value) selectQuery.Append($"'{itemValue}',");

            return
                $"{selectQuery.ToString().Substring(0, selectQuery.Length - 1)}) AND \"IsDeleted\" is false ORDER BY \"CreationDate\" DESC";
        }

        public string GenerateSelectInQuery<TEntity>(KeyValuePair<string, List<string>> criteria, int pageSize,
            int pageNumber) where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var selectQuery = new StringBuilder($"SELECT * FROM {tableName} WHERE \"{criteria.Key}\" IN (");

            foreach (var itemValue in criteria.Value) selectQuery.Append($"'{itemValue}',");

            return
                $"{selectQuery.ToString().Substring(0, selectQuery.Length - 1)}) AND \"IsDeleted\" is false ORDER BY \"CreationDate\" DESC LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";
        }

        public string GenerateSelectCountQuery<TEntity>() where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var selectQuery = new StringBuilder($"SELECT COUNT(1) from {tableName} ");
            return selectQuery.ToString();
        }

        public string GenerateSelectCountWhereQuery<TEntity>(Dictionary<string, string> criteria) where TEntity : class
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var selectQuery = new StringBuilder($"SELECT COUNT(1) from {tableName} ");
            var count = 1;
            criteria.TryAdd("IsDeleted", "false");

            if (criteria.Any())
            {
                selectQuery.Append(" WHERE ");
                foreach (var item in criteria)
                {
                    var columnName = char.ToUpper(item.Key[0]) + item.Key.Substring(1);
                    selectQuery.Append($"\"{columnName}\" = '{item.Value}' ");
                    if (criteria.Count > count) selectQuery.Append("AND ");

                    count++;
                }
            }

            return selectQuery.ToString();
        }

        public string GenerateSelectCountInAsync<TEntity>(KeyValuePair<string, List<string>> criteria)
        {
            var tableName = typeof(TEntity).GetReadTableName();
            var selectQuery = new StringBuilder($"SELECT COUNT(1) FROM {tableName} WHERE \"{criteria.Key}\" in (");
            foreach (var itemValue in criteria.Value) selectQuery.Append($"'{itemValue}',");
            return $"{selectQuery.ToString().Substring(0, selectQuery.Length - 1)}) AND \"IsDeleted\" is false";
        }
    }
}
