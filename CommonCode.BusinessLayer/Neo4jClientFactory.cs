using System;
using System.Data;
using CommonCode.BusinessLayer.Helpers;
using Neo4jClient;

namespace CommonCode.BusinessLayer
{
    public class Neo4JClientFactory
    {
        private readonly GraphClient _client;

        public Neo4JClientFactory(string uri, string user, string password)
        {
            Verify.NotNull(uri, nameof(uri));
            Verify.NotNull(user, nameof(user));
            Verify.NotNull(password, nameof(password));

            _client = new GraphClient(new Uri(uri), user, password);
        }

        public IGraphClient Make(string connectionString = null, IDbTransaction transaction = null)
        {
            return _client;
        }
    }
}