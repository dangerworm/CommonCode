using System.Collections.Generic;

namespace CommonCode.BusinessLayer
{
    public interface IUnitOfWork<TConnection, TTransaction>
    {
        // Connection handling
        TConnection GetConnection();
        bool HasConnectionOrSession();
        IUnitOfWork<TConnection, TTransaction> Begin();
        void End();

        // Transaction handling
        TTransaction GetTransaction();
        void BeginTransaction();
        bool IsSuccess();
        void Commit();
        void Rollback();
        void EndTransaction();

        // DataResults
        void AddDataResult(DataResult result);
        IReadOnlyCollection<DataResult> GetAllDataResults();
        DataResult GetLastDataResult();

        void Dispose();
    }
}