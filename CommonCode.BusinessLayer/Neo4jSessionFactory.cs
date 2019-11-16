using System;
using System.Data;
using CommonCode.BusinessLayer.Helpers;
using Neo4j.Driver.V1;
using Neo4jClient;

namespace Curio.ServiceLayer
{
    public class Neo4JSessionFactory
    {
        private readonly IDriver _driver;

        public Neo4JSessionFactory(string uri, string user, string password)
        {
            Verify.NotNull(uri, nameof(uri));
            Verify.NotNull(user, nameof(user));
            Verify.NotNull(password, nameof(password));

            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public ISession Make(string connectionString = null, IDbTransaction transaction = null)
        {
            var session = _driver.Session();

            return session;
        }
    }
}