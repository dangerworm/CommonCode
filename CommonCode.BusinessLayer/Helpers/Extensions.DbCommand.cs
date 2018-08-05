using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CommonCode.BusinessLayer.Helpers
{
    public static partial class Extensions
    {
        public static IDbDataParameter AddParameter(this IDbCommand command, string name, IEnumerable<int> value)
        {
            Verify.ValidString(name, nameof(name));

            var parameter = new SqlParameter(name, BuildTableValuedParameters.BuildTable(value))
            {
                SqlDbType = SqlDbType.Structured
            };

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbDataParameter AddParameter<TEnum>(this IDbCommand command, string name, IEnumerable<TEnum> value)
            where TEnum : struct
        {
            Verify.ValidString(name, nameof(name));

            var parameter = new SqlParameter(name, BuildTableValuedParameters.BuildTable(value))
            {
                SqlDbType = SqlDbType.Structured
            };

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbDataParameter AddParameter(this IDbCommand command, string name, DbType parameterType, ParameterDirection? direction = null, int? size = null)
        {
            Verify.ValidString(name, nameof(name));

            if (parameterType == DbType.Time)
            {
                var sqlParameter = new SqlParameter(name, SqlDbType.Time);
                if (direction.HasValue)
                {
                    sqlParameter.Direction = direction.Value;
                }
                if (size.HasValue)
                {
                    sqlParameter.Size = size.Value;
                }

                command.Parameters.Add(sqlParameter);

                return sqlParameter;
            }

            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = parameterType;

            if (direction.HasValue)
            {
                parameter.Direction = direction.Value;
            }

            if (size.HasValue)
            {
                parameter.Size = size.Value;
            }

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbDataParameter AddParameter(this IDbCommand command, string name, DbType parameterType, ParameterDirection direction, object parameterValue)
        {
            Verify.ValidString(name, nameof(name));

            if (parameterType == DbType.Time)
            {
                var sqlParameter = new SqlParameter(name, SqlDbType.Time)
                {
                    Direction = direction,
                    Value = parameterValue
                };

                command.Parameters.Add(sqlParameter);

                return sqlParameter;
            }

            var parameter = command.CreateParameter();
            parameter.DbType = parameterType;
            parameter.ParameterName = name;
            parameter.Direction = direction;
            parameter.SetValue(parameterValue);

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbCommand AddWithValue(this IDbCommand command, string name, DbType parameterType, object parameterValue)
        {
            Verify.ValidString(name, nameof(name));

            if (parameterValue == null)
            {
                parameterValue = DBNull.Value;
            }

            command.AddParameter(name, parameterType).SetValue(parameterValue);

            return command;
        }

        public static IDbCommand AddIntTable(this IDbCommand command, string name, IEnumerable<int> values)
        {
            Verify.ValidString(name, nameof(name));

            var parameter = new SqlParameter(name, BuildTableValuedParameters.BuildTable(values ?? new List<int>()))
            {
                SqlDbType = SqlDbType.Structured
            };

            command.Parameters.Add(parameter);

            return command;
        }

        public static IDbCommand AddKeyValueTable<TId, TValue>(this IDbCommand command, string name, IEnumerable<KeyValuePair<TId, TValue>> list)
        {
            var statusParam = new SqlParameter(name, BuildTableValuedParameters.BuildTable(list ?? Enumerable.Empty<KeyValuePair<TId, TValue>>()))
            {
                SqlDbType = SqlDbType.Structured
            };

            command.Parameters.Add(statusParam);

            return command;
        }

        public static IDbDataParameter AddReturnValue(this IDbCommand command)
        {
            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.ReturnValue;

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbDataParameter AddOutput(this IDbCommand command, string name, DbType parameterType, object parameterValue = null)
        {
            if (parameterType != DbType.Time)
            {
                return parameterValue == null
                    ? command.AddParameter(name, parameterType, ParameterDirection.Output)
                    : command.AddParameter(name, parameterType, ParameterDirection.InputOutput, parameterValue);
            }

            var sqlParameter = new SqlParameter(name, SqlDbType.Time)
            {
                Direction = ParameterDirection.InputOutput
            };

            if (parameterValue != null)
            {
                sqlParameter.Value = parameterValue;
            }

            command.Parameters.Add(sqlParameter);

            return sqlParameter;
        }

        public static string GetDetails(this IDbCommand command)
        {
            var result = new StringBuilder();

            result.AppendLine();
            result.AppendLine("Command details:");
            result.AppendLine();
            result.AppendLine($"Connection String: {command.Connection.ConnectionString}");
            result.AppendLine($"Is using transaction: {command.Transaction != null}");
            result.AppendLine($"Command Type: {command.CommandType}");
            result.AppendLine($"Command Text: {command.CommandText}");
            result.AppendLine();
            result.AppendLine("Command Parameters:");
            result.AppendLine();

            foreach (IDbDataParameter parameter in command.Parameters)
            {
                result.AppendFormat("Name: {0} | ", parameter.ParameterName);
                result.AppendFormat("Type: {0} | ", parameter.DbType);
                result.AppendFormat("Direction: {0} | ", parameter.Direction);
                result.AppendFormat("Value: {0}{1}", parameter.Value, Environment.NewLine);
            }

            return result.ToString();
        }
    }
}