

using Music_Booking_App.Core.Attibutes;

namespace Music_Booking_App.Data.Extensions
{
    public static class Extension
    {
        public static bool IgnoreProperty(this Type typeObject, string propertyName)
        {
            var property = typeObject.GetProperty(propertyName);

            if (property is null)
                throw new InvalidOperationException("Property to be ignored cannot be null.");

            return Attribute.IsDefined(property, typeof(IgnoreDuringInsertOrUpdateAttribute), false);
        }

        public static string? GetReadTableName(this Type entity)
        {
            if (!Attribute.IsDefined(entity, typeof(ReadTableNameAttribute), false))
                return null;

            var tableNameAttribute =
                (ReadTableNameAttribute?)Attribute.GetCustomAttribute(entity, typeof(ReadTableNameAttribute));

            return tableNameAttribute?.Name;
        }

        public static string? GetWriteTableName(this Type entity)
        {
            if (!Attribute.IsDefined(entity, typeof(WriteTableNameAttribute), false))
                return null;

            var tableNameAttribute =
                (WriteTableNameAttribute?)Attribute.GetCustomAttribute(entity, typeof(WriteTableNameAttribute));

            return tableNameAttribute?.Name;
        }
    }
}
