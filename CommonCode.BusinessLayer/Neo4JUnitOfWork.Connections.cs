using System;
using System.Collections.Generic;

namespace CommonCode.BusinessLayer
{
    public partial class Neo4JUnitOfWork<TConnection, TTransaction>
    {
        public TConnection GetConnection()
        {
            if (_client == null || !_client.IsConnected)
            {
                throw new InvalidOperationException("You must call Begin() before you can access the connection.");
            }

            return (TConnection)_client;
        }

        public bool HasConnectionOrSession()
        {
            return _client != null && _client.IsConnected;
        }

        public IUnitOfWork<TConnection, TTransaction> Begin()
        {
            if (_client != null && _client.IsConnected)
            {
                return this;
                //throw new InvalidOperationException("Unit of Work has already been started. You must call End() before calling Begin() again.");
            }

            _client = _clientFactory.Make();
            _client.Connect();
            _results = new List<DataResult>();

            return this;
        }

        public void End()
        {
            if (_client == null || !_client.IsConnected)
            {
                throw new InvalidOperationException(
                    "Unit of work has not been started. You must call Begin() before calling End().");
            }

            if (_transaction != null)
            {
                EndTransaction();
            }

            _client.Dispose();
            _client = null;
        }
    }
}
