using CommonCode.BusinessLayer.Helpers;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CommonCode.BusinessLayer.Repositories
{
    public abstract partial class SqlRepositoryBase<T>
    {
        protected string Schema { get; set; }
        protected string TableName { get; set; }

        protected SqlRepositoryBase(IUnitOfWork<IDbConnection, IDbTransaction> unitOfWork, string tableName, string schema = "dbo")
        {
            Verify.NotNull(unitOfWork, nameof(unitOfWork));
            Verify.ValidString(tableName, nameof(tableName));

            UnitOfWork = unitOfWork;
            TableName = tableName;
            Schema = schema;
        }

        protected void Initialise(string tableName)
        {
            Verify.ValidString(tableName, nameof(tableName));

            TableName = tableName;
        }

        public virtual DataResult<T> Create(T value)
        {
            Verify.ValidString(TableName, nameof(TableName));

            var storedProcedureName = $"{Schema}.USP_{TableName}_Create";
            var parameters = new DynamicParameters();

            AddCommonParameters(ref parameters, value);

            return Read(storedProcedureName, parameters);
        }

        public virtual DataResult<IEnumerable<T>> GetAll()
        {
            Verify.ValidString(TableName, nameof(TableName));

            var storedProcedureName = $"{Schema}.USP_{TableName}_GetAll";
            var parameters = new DynamicParameters();

            return ReadList(storedProcedureName, parameters);
        }

        public virtual DataResult<T> GetById(int id)
        {
            Verify.ValidString(TableName, nameof(TableName));

            var storedProcedureName = $"{Schema}.USP_{TableName}_GetById";
            var parameters = new DynamicParameters();

            parameters.Add("@Id", id, DbType.Int32);

            return Read(storedProcedureName, parameters);
        }

        public virtual DataResult<T> Update(T value)
        {
            var storedProcedureName = $"{Schema}.USP_{TableName}_Update";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", value.Id, DbType.Int32);
            AddCommonParameters(ref parameters, value);

            return Read(storedProcedureName, parameters);
        }

        public virtual DataResult Delete(int id)
        {
            var storedProcedureName = $"{Schema}.USP_{TableName}_Delete";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);

            return Delete(storedProcedureName, parameters);
        }

        protected virtual void AddCommonParameters(ref DynamicParameters parameters, T value)
        {
            AddCommonParameters(ref parameters, value, null);
        }

        protected virtual void AddCommonParameters(ref DynamicParameters parameters,
            T value, IEnumerable<string> propertyNames)
        {
            var properties = typeof(T).GetProperties();

            if (propertyNames != null)
            {
                properties = properties.Where(p => propertyNames.Contains(p.Name)).ToArray();
            }

            foreach (var property in properties)
            {
                if (property.Name.Equals("Id"))
                {
                    continue;
                }

                DbType dbType;
                switch (property.PropertyType.Name)
                {
                    case "Boolean":
                        dbType = DbType.Boolean;
                        break;

                    case "DateTime":
                        dbType = DbType.DateTime2;
                        break;

                    case "Decimal":
                        dbType = DbType.Decimal;
                        break;

                    case "Double":
                        dbType = DbType.Double;
                        break;

                    case "Guid":
                        dbType = DbType.Guid;
                        break;

                    case "Int32":
                        dbType = DbType.Int32;
                        break;

                    case "String":
                        dbType = DbType.String;
                        break;

                    default:
                        dbType = DbType.Double;
                        break;
                }

                parameters.Add($"@{property.Name}", property.GetGetMethod().Invoke(value, null), dbType);
            }
        }
    }
}