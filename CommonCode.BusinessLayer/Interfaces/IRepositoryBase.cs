using System.Collections.Generic;
using CommonCode.BusinessLayer.POCOs;
using Dapper;

namespace CommonCode.BusinessLayer.Interfaces
{
    public interface IRepositoryBase<T> where T : IPoco
    {
        DataResult<T> Create(T value);
        DataResult Delete(int id);
        DataResult Delete(string storedProcedureName, DynamicParameters parameters);
        DataResult<IEnumerable<T>> GetAll();
        DataResult<T> GetById(int id);
        DataResult<T> Update(T value);
    }
}