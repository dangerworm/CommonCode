using System;
using System.Linq;
using CommonCode.BusinessLayer.Helpers;
using Neo4jClient.Transactions;

namespace CommonCode.BusinessLayer
{
    public partial class Neo4JUnitOfWork<TConnection, TTransaction>
    {
        public TTransaction GetTransaction()
        {
            return _transaction;
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException(
                    "Transaction has already been started. You must call EndTransaction() before calling BeginTransaction() again.");
            }

            _transaction = (TTransaction) ((ITransactionalGraphClient)GetConnection()).BeginTransaction();
        }

        public bool IsSuccess()
        {
            return _results.All(x => x.Type.IsAny(DataResultType.Success, DataResultType.NotRequired));
        }

        private void FinaliseTransaction()
        {
            _transaction.Dispose();
            _transaction = default(TTransaction);
            _results.Clear();
        }

        public void Commit()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("You must call BeginTransaction() before Commit().");
            }

            _transaction.Success();
            FinaliseTransaction();
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("You must call BeginTransaction() before Rollback().");
            }

            _transaction.Failure();
            FinaliseTransaction();
        }

        public void EndTransaction()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("You must call BeginTransaction() before EndTransaction().");
            }

            if (IsSuccess())
            {
                Commit();
            }
            else
            {
                Rollback();
            }
        }
    }
}