using System;
using System.Data;

namespace CommonCode.BusinessLayer.Helpers
{
    public static partial class Extensions
    {
        public static void SetValue(this IDbDataParameter parameter, object value)
        {
            parameter.Value = value ?? DBNull.Value;
        }

        public static void SetValue<T>(this IDbDataParameter parameter, T? value)
            where T : struct
        {
            if (value.HasValue)
            {
                parameter.Value = value.Value;
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}