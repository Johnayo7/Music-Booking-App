
using Music_Booking_App.Core.Helpers;
using Music_Booking_App.Data.Extensions;
using Music_Booking_App.Data.Helpers.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace Music_Booking_App.Data.Helpers.Implementations
{
    public class WriteUtilities : IWriteUtilities
    {
        public object GetObjectParams<TEntity>(TEntity entity) where TEntity : class
        {
            return GenerateParams(typeof(TEntity).GetProperties(), entity).ConvertToAnonymousObject();
        }

        public string GenerateInsertQuery<TEntity>(TEntity entity) where TEntity : class
        {
            var tableName = typeof(TEntity).GetWriteTableName();

            var insertQuery = new StringBuilder($"INSERT INTO {tableName} ");
            insertQuery.Append('(');
            var properties = GenerateParams(typeof(TEntity).GetProperties(), entity).Keys.ToList();
            properties.ForEach(prop => { insertQuery.Append("\"" + prop + "\","); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(");");
            return insertQuery.ToString();
        }

        public string GenerateUpdateQuery<TEntity>(TEntity entity) where TEntity : class
        {
            var tableName = typeof(TEntity).GetWriteTableName();
            var updateQuery = new StringBuilder($"UPDATE {tableName} ");
            updateQuery.Append("SET");
            var properties = GenerateParams(typeof(TEntity).GetProperties(), entity).Keys.ToList();
            foreach (var prop in properties)
            {
                if (prop is "Id" or "CustomerId") continue;
                updateQuery.Append($" \"{prop}\" = @{prop},");
            }

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" where \"Id\" = @Id");
            return updateQuery.ToString();
        }

        private static Dictionary<string, object?> GenerateParams<TEntity>(IEnumerable<PropertyInfo> properties,
            TEntity entity) where TEntity : class
        {
            var objectDictionary = new Dictionary<string, object?>();
            foreach (var property in properties)
            {
                if (entity.GetType().IgnoreProperty(property.Name))
                    continue;

                var attribute = Array.Find(property
                    .GetCustomAttributes(true), a => a.GetType() == typeof(ColumnAttribute));

                if (attribute != null)
                {
                    var column = ((ColumnAttribute)attribute).Name;
                    objectDictionary.Add(!string.IsNullOrWhiteSpace(column) ? column : property.Name,
                        property.GetValue(entity));
                }
                else
                {
                    objectDictionary.Add(property.Name, property.GetValue(entity));
                }
            }

            return objectDictionary;
        }
    }
}
