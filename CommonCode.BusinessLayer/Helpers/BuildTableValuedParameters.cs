using System;
using System.Collections.Generic;
using System.Data;

namespace CommonCode.BusinessLayer.Helpers
{
    public static class BuildTableValuedParameters
    {
        private static DataTable GetIntListTable()
        {
            var table = new DataTable("List");
            table.Columns.Add("Id", typeof(int));

            return table;
        }

        public static DataTable BuildTable(IEnumerable<int> integers)
        {
            var table = GetIntListTable();

            foreach (var integer in integers)
            {
                table.Rows.Add(integer);
            }

            return table;
        }

        public static DataTable BuildTable<T>(IEnumerable<T> values)
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("Values must be a type of enum");
            }

            var table = GetIntListTable();

            foreach (var value in values)
            {
                table.Rows.Add(value);
            }

            return table;
        }

        public static DataTable BuildTable<TId, TValue>(IEnumerable<KeyValuePair<TId, TValue>> valuePairs)
        {
            var table = GetIdValueListTable<TId, TValue>();

            foreach (var pair in valuePairs)
            {
                var row = table.NewRow();
                row["Id"] = pair.Key;
                row["Value"] = pair.Value;

                table.Rows.Add(row);
            }
            return table;
        }

        private static DataTable GetIdValueListTable<TId, TValue>()
        {
            var table = new DataTable("List");
            table.Columns.Add("Id", typeof(TId));
            table.Columns.Add("Value", typeof(TValue));

            return table;
        }
    }
}