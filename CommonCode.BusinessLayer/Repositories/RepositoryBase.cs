using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using CommonCode.BusinessLayer.Helpers;
using Dapper;

namespace CommonCode.BusinessLayer.Repositories
{
    public abstract partial class RepositoryBase<T>
    {
        private const string FriendlyReadMessage = "The system was unable to read the requested item(s) from the database.";
        private const string InternalReadMessage = "A database exception occurred when trying to read the value(s).";
        private const string Success = "Success";

        protected IUnitOfWork UnitOfWork { get; }
        protected bool HasCustomReader { get; set; }

        protected RepositoryBase(IUnitOfWork unitOfWork, bool hasCustomReader)
        {
            Verify.NotNull(unitOfWork, nameof(unitOfWork));

            UnitOfWork = unitOfWork;
            HasCustomReader = hasCustomReader;
        }

        protected DataResult<T> Read(string storedProcedureName, DynamicParameters parameters)
        {
            parameters = parameters ?? new DynamicParameters();
            parameters.Add("RowCount", null, DbType.Int32, ParameterDirection.ReturnValue);

            try
            {
                var connection = UnitOfWork.GetConnection();

                var value = connection.QueryFirstOrDefault<T>(
                storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

                var rowCount = parameters.Get<int>("RowCount");

                return CreateDataResult(storedProcedureName, rowCount, value, DataResultType.Success, Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(storedProcedureName, 0, default(T), DataResultType.UnknownError, FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        protected DataResult<IEnumerable<T>> ReadList(string storedProcedureName, DynamicParameters parameters)
        {
            parameters = parameters ?? new DynamicParameters();
            parameters.Add("RowCount", null, DbType.Int32, ParameterDirection.ReturnValue);

            try
            {
                var connection = UnitOfWork.GetConnection();

                var values = connection.Query<T>(
                storedProcedureName, parameters, commandType: CommandType.StoredProcedure);

                var rowCount = parameters.Get<int>("RowCount");

                return CreateDataResult(storedProcedureName, rowCount, values, DataResultType.Success, Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(storedProcedureName, 0, default(T), DataResultType.UnknownError, FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        public DataResult Delete(string storedProcedureName, DynamicParameters parameters)
        {
            return ExecuteStoredProcedure(storedProcedureName, parameters);
        }

        protected DataResult ExecuteStoredProcedure(string storedProcedureName, DynamicParameters parameters)
        {
            DataResultType resultType;
            string commandType;
            string commandTypePastTense;
            string friendlyMessage;
            string internalMessage;
            InitialiseInformativeVariables(storedProcedureName, out commandType, out commandTypePastTense,
            out resultType, out friendlyMessage, out internalMessage);

            var rowCount = 0;
            DbException dbException = null;

            try
            {
                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }

                parameters.Add("RowCount", null, DbType.Int32, ParameterDirection.ReturnValue);

                using (var connection = UnitOfWork.GetConnection())
                {
                    connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                }

                rowCount = parameters.Get<int>("RowCount");

                if (rowCount > 0)
                {
                    resultType = DataResultType.Success;
                    friendlyMessage = "Success";
                    internalMessage = $"Record(s) {commandTypePastTense} successfully.";
                }
                else
                {
                    friendlyMessage = $"Sorry, the database was unable to {commandType} the data.";
                    internalMessage = $"Unable to {commandType} record. No records affected.";
                }
            }
            catch (DbException exception)
            {
                friendlyMessage = $"Sorry, the database was unable to {commandType} the data.";
                internalMessage = $"Unable to {commandType} record. An exception occurred with the database." +
                $"{Environment.NewLine}{exception.Message}";
                dbException = exception;
            }

            return CreateDataResult(storedProcedureName, rowCount, default(T), resultType, friendlyMessage, internalMessage, null, dbException);
        }

        protected static void InitialiseInformativeVariables(string commandText, out string commandType,
        out string commandTypePastTense, out DataResultType resultType, out string friendlyMessage, out string internalMessage)
        {
            commandType = "process";
            resultType = DataResultType.UnknownError;

            if (commandText.EndsWith("create"))
            {
                commandType = "create";
                resultType = DataResultType.UnableToCreateRecord;
            }
            else if (commandText.Contains("get"))
            {
                commandType = "read";
                resultType = DataResultType.UnableToReadRecord;
            }
            else if (commandText.EndsWith("save"))
            {
                commandType = "save";
                resultType = DataResultType.UnableToUpdateRecord;
            }
            else if (commandText.EndsWith("update"))
            {
                commandType = "update";
                resultType = DataResultType.UnableToUpdateRecord;
            }
            else if (commandText.EndsWith("delete"))
            {
                commandType = "delete";
                resultType = DataResultType.UnableToDeleteRecord;
            }

            if (commandType.Equals("process"))
            {
                commandTypePastTense = $"{commandType}ed";
            }
            else if (!commandType.Equals("read"))
            {
                commandTypePastTense = $"{commandType}d";
            }
            else
            {
                commandTypePastTense = commandType;
            }

            friendlyMessage = "Sorry, an unexpected error has occurred.";
            internalMessage = $"Unable to {commandType} record. No records affected.";
        }

        protected DataResult<TResult> CreateDataResult<TResult>(string storedProcedureName, int rowCount,
        TResult value, DataResultType resultType, string friendlyMessage, string internalMessage,
        int? valueId = null, DbException exception = null)
        {
            if (rowCount > 0)
            {
                resultType = DataResultType.Success;
                friendlyMessage = !string.IsNullOrWhiteSpace(friendlyMessage) ? friendlyMessage : Success;
                internalMessage = !string.IsNullOrWhiteSpace(internalMessage) ? internalMessage : $"Procedure {storedProcedureName} completed successfully.";
            }
            else if (resultType.Equals(DataResultType.Success) && !storedProcedureName.ToLower().Contains("get"))
            {
                resultType = DataResultType.NotRequired;
                friendlyMessage = !string.IsNullOrWhiteSpace(friendlyMessage) && !friendlyMessage.Equals(Success) ? friendlyMessage : "No actions required";
                internalMessage = !string.IsNullOrWhiteSpace(internalMessage) ? internalMessage : $"Procedure {storedProcedureName} did not require any changes.";
            }
            else
            {
                friendlyMessage = "Sorry, the action did not complete successfully.";
                internalMessage = $"Procedure {storedProcedureName} failed. No records affected.";
            }

            var dataResult = new DataResult<TResult>(value, resultType, friendlyMessage, internalMessage, exception);
            dataResult.Data.Add("Command Details", storedProcedureName);

            if (valueId.HasValue)
            {
                dataResult.ValueId = valueId.Value;
            }

            exception?.Data.Add("ExceptionData.StringValue", storedProcedureName);

            UnitOfWork.AddDataResult(dataResult);

            return dataResult;
        }
    }
}