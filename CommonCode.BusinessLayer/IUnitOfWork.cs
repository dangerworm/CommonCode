using System.Collections.Generic;
using System.Data;

namespace CommonCode.BusinessLayer
{
    public interface IUnitOfWork
    {
        // Connection handling
        IDbConnection GetConnection();
        bool HasConnection();
        UnitOfWork Begin();
        void End();

        // Transaction handling
        IDbTransaction GetTransaction();
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