using System;
using System.Collections.Generic;
using System.Data;

namespace CommonCode.BusinessLayer
{
    public partial class UnitOfWork
    {
        public IDbConnection GetConnection()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("You must call Begin() before you can access the connection.");
            }

            return _connection;
        }

        public bool HasConnection()
        {
            return _connection != null;
        }

        public UnitOfWork Begin()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                return this;
                //throw new InvalidOperationException("Unit of Work has already been started. You must call End() before calling Begin() again.");
            }

            _connection = _connectionFactory.Make(_connectionString);
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
            _connection = null;
        }
    }
}