using CommonCode.BusinessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CommonCode.BusinessLayer
{
    public partial class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly string _connectionString;
        private readonly string _id;

        private IDbConnection _connection;
        private List<DataResult> _results;

        private SqlTransaction _transaction;

        public UnitOfWork(string connectionString)
        {
            Verify.NotNull(connectionString, nameof(connectionString));

            _connectionFactory = new DbConnectionFactory();
            _connectionString = connectionString;
            _id = Guid.NewGuid().ToString();
        }

        public void AddDataResult(DataResult result)
        {
            Verify.NotNull(result, nameof(result));

            _results.Add(result);
        }

        public IReadOnlyCollection<DataResult> GetAllDataResults()
        {
            return new ReadOnlyCollection<DataResult>(_results);
        }

        public DataResult GetLastDataResult()
        {
            return _results.LastOrDefault();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                EndTransaction();
            }

            if (_connection != null)
            {
                End();
            }
        }

        public override string ToString()
        {
            return "UnitOfWork-" + _id;
        }
    }
}