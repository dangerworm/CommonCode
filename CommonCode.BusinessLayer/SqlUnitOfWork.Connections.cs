using System;
using System.Collections.Generic;
using System.Data;

namespace CommonCode.BusinessLayer
{
    public partial class SqlUnitOfWork<TConnection, TTransaction>
    {
        public TConnection GetConnection()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("You must call Begin() before you can access the connection.");
            }

            // Dapper closes the connection after each call.
            // We can tell the difference between our operations and Dapper's because
            // Dapper closes the connection and sets the ConnectionString to "" whereas
            // we close and set _connection to null. So if there's no connection string
            // it probably means we need to make another call, so open the connection.
            if (_connection.ConnectionString == string.Empty)
            {
                _connection = (TConnection)_connectionFactory.Make(_connectionString);
                _connection.Open();
            }

            return _connection;
        }

        public bool HasConnectionOrSession()
        {
            return _connection != null;
        }

        public IUnitOfWork<TConnection, TTransaction> Begin()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                return this;
                //throw new InvalidOperationException("Unit of Work has already been started. You must call End() before calling Begin() again.");
            }

            _connection = (TConnection)_connectionFactory.Make(_connectionString);
            _connection.Open();

            _results = new List<DataResult>();
            return this;
        }

        public void End()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException(
                    "Unit of work has not been started. You must call Begin() before calling End().");
            }

            if (_connection.State != ConnectionState.Open)
            {
                if (_transaction != null)
                {
                    EndTransaction();
                }

                _connection.Close();
            }

            _connection.Dispose();
            _connection = default(TConnection);
        }
    }
}
