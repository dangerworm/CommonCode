using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonCode.BusinessLayer.Helpers;
using Curio.ServiceLayer;
using Neo4j.Driver.V1;
using Neo4jClient;

namespace CommonCode.BusinessLayer
{
    public partial class Neo4JUnitOfWork<TConnection, TTransaction> 
        : IUnitOfWork<TConnection, TTransaction>
            where TConnection : IGraphClient
            where TTransaction : ITransaction
    {
        private readonly Neo4JClientFactory _clientFactory;
        private readonly string _id;

        private IGraphClient _client;
        private List<DataResult> _results;

        private TTransaction _transaction;

        public Neo4JUnitOfWork(string uri, string user, string password)
        {
            Verify.NotNull(uri, nameof(uri));
            Verify.NotNull(user, nameof(user));
            Verify.NotNull(password, nameof(password));

            _clientFactory = new Neo4JClientFactory(uri, user, password);
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
            if (_client != null)
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
