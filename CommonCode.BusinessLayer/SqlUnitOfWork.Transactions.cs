using CommonCode.BusinessLayer.Helpers;
using System;
using System.Linq;

namespace CommonCode.BusinessLayer
{
    public partial class SqlUnitOfWork<TConnection, TTransaction>
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

            _transaction = (TTransaction)GetConnection().BeginTransaction();
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

            _transaction.Commit();
            FinaliseTransaction();
        }

        public void Rollback()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("You must call BeginTransaction() before Rollback().");
            }

            _transaction.Rollback();
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