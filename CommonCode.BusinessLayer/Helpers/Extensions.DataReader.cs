using System;
using System.Data;

namespace CommonCode.BusinessLayer.Helpers
{
    public static partial class Extensions
    {
        public static T Get<T>(this IDataRecord reader, int columnIndex)
        {
            return Get(reader, columnIndex, default(T));
        }

        public static T Get<T>(this IDataRecord reader, Enum enumColumnIndex)
        {
            return Get(reader, enumColumnIndex, default(T));
        }

        public static T Get<T>(this IDataRecord reader, Enum enumColumnIndex, T nullValue)
        {
            return Get(reader, Convert.ToInt32(enumColumnIndex), nullValue, enumColumnIndex.ToString());
        }

        public static T Get<T>(this IDataRecord reader, int columnIndex, T nullValue)
        {
            return Get(reader, columnIndex, nullValue, null);
        }

        private static T Get<T>(IDataRecord reader, int columnIndex, T nullValue, string columnName)
        {
            if (reader.IsDBNull(columnIndex))
            {
                return nullValue;
            }

            // SQL char type is a string. Need to box and unbox to support generic
            if (typeof(T) == typeof(char) || typeof(T) == typeof(char?))
            {
                var stringResult = reader.GetString(columnIndex);

                if (stringResult.Length == 1)
                {
                    return (T)(object)stringResult[0];
                }

                return nullValue;
            }

            try
            {
                return (T)reader[columnIndex];
            }
            catch (InvalidCastException exception)
            {
                throw new InvalidOperationException("Requested type does not match SQL data type. " +
                                                    $"Column: {columnName ?? columnIndex.ToString()}, Attempted Type: {typeof(T).Name}.",
                    exception);
            }
        }
    }
}