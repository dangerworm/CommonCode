using System.Collections.Generic;
using Dapper;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IRepositoryBase<T> where T : IIdentifiableByInteger
    {
        DataResult<T> Create(T value);
        DataResult Delete(int id);
        DataResult Delete(string storedProcedureName, DynamicParameters parameters);
        DataResult<IEnumerable<T>> GetAll();
        DataResult<T> GetById(int id);
        DataResult<T> Update(T value);
    }
}