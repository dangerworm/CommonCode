using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CommonCode.BusinessLayer
{
    public class DbConnectionFactory
    {
        private readonly DbProviderFactory _dbProviderFactory;

        public DbConnectionFactory(string provider = "System.Data.SqlClient")
        {
            try
            {
                _dbProviderFactory = DbProviderFactories.GetFactory(provider);
            }
            catch (Exception)
            {
                Console.WriteLine($"{provider} is not a valid DB provider.");
                throw;
            }
        }

        public IDbConnection Make(string connectionString = null, IDbTransaction transaction = null)
        {
            if (transaction != null)
            {
                return transaction.Connection;
            }

            var connection = (SqlConnection)_dbProviderFactory.CreateConnection();

            if (connection != null && !string.IsNullOrWhiteSpace(connectionString))
            {
                connection.ConnectionString = connectionString;
            }

            return connection;
        }

        public DbConnectionStringBuilder GetConnectionStringBuilder(string connectionString = null)
        {
            var builder = _dbProviderFactory.CreateConnectionStringBuilder();

            if (builder != null && !string.IsNullOrWhiteSpace(connectionString))
            {
                builder.ConnectionString = connectionString;
            }

            return builder;
        }
    }
}