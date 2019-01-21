using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using CommonCode.BusinessLayer.Helpers;
using Neo4j.Driver.V1;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace CommonCode.BusinessLayer.Repositories
{
    public abstract class Neo4JRepositoryBase<T>
    {
        public const string Answer = "Answer";
        public const string Concept = "Concept";
        public const string Question = "Question";
        public const string SystemLogin = "SystemLogin";
        public const string User = "User";

        private const string FriendlyReadMessage = "The system was unable to read the requested item(s) from the database.";
        private const string InternalReadMessage = "A database exception occurred when trying to read the value(s).";
        protected const string Success = "Success";

        protected IUnitOfWork<IGraphClient, ITransaction> UnitOfWork { get; }
        protected bool HasCustomReader { get; set; }

        protected Neo4JRepositoryBase(IUnitOfWork<IGraphClient, ITransaction> unitOfWork, bool hasCustomReader = false)
        {
            Verify.NotNull(unitOfWork, nameof(unitOfWork));

            UnitOfWork = unitOfWork;
            HasCustomReader = hasCustomReader;
        }

        protected virtual T Map(ICypherFluentQuery<T> query)
        {
            throw new NotImplementedException("HasCustomReader is set to true but Map has not been implemented.");
        }

        protected virtual IEnumerable<T> MapList(ICypherFluentQuery<T> query)
        {
            throw new NotImplementedException("HasCustomReader is set to true but MapList has not been implemented.");
        }

        protected DataResult<T> Read(ICypherFluentQuery<T> query)
        {
            if (HasCustomReader)
            {
                return Read(query, Map);
            }

            try
            {
                var result = query.ResultsAsync.Result;

                var value = result.SingleOrDefault();
                var rowCount = value == null ? 0 : 1;

                return CreateDataResult(query.Query.QueryText, rowCount, 
                    value, DataResultType.Success, Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(query.Query.QueryText, 0, default(T), DataResultType.UnknownError, 
                    FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        protected DataResult<IEnumerable<T>> ReadList(ICypherFluentQuery<T> query)
        {
            if (HasCustomReader)
            {
                return ReadList(query, MapList);
            }

            try
            {
                var values = query.Results;

                var rowCount = values.Count();

                return CreateDataResult(query.Query.QueryText, rowCount, values, DataResultType.Success, 
                    Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(query.Query.QueryText, 0, default(T), DataResultType.UnknownError, 
                    FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        protected DataResult<T> Read(ICypherFluentQuery<T> query, Func<ICypherFluentQuery<T>, T> map)
        {
            try
            {
                var value = map(query);
                var itemCount = value == null ? 0 : 1;

                return CreateDataResult(query.Query.QueryText, itemCount, value, DataResultType.Success,
                    Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(query.Query.QueryText, 0, default(T), DataResultType.UnknownError,
                    FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        protected DataResult<IEnumerable<T>> ReadList(ICypherFluentQuery<T> query, Func<ICypherFluentQuery<T>, IEnumerable<T>> map)
        {
            try
            {
                var values = map(query);
                var itemCount = ((ICollection) values).Count;

                return CreateDataResult(query.Query.QueryText, itemCount, values, DataResultType.Success,
                    Success, Success);
            }
            catch (DbException exception)
            {
                CreateDataResult(query.Query.QueryText, 0, default(T), DataResultType.UnknownError,
                    FriendlyReadMessage, InternalReadMessage, null, exception);
                throw;
            }
        }

        public DataResult Delete(string functionName, List<string> parameters)
        {
            throw new NotImplementedException();
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

        protected DataResult<TResult> CreateDataResult<TResult>(string functionName, int rowCount,
        TResult value, DataResultType resultType, string friendlyMessage, string internalMessage,
        int? valueId = null, DbException exception = null)
        {
            if (rowCount > 0)
            {
                resultType = DataResultType.Success;
                friendlyMessage = !string.IsNullOrWhiteSpace(friendlyMessage) ? friendlyMessage : Success;
                internalMessage = !string.IsNullOrWhiteSpace(internalMessage) ? internalMessage : $"Procedure {functionName} completed successfully.";
            }
            else if (resultType.Equals(DataResultType.Success) && !functionName.ToLower().Contains("get") && !functionName.ToLower().Contains("read"))
            {
                resultType = DataResultType.NotRequired;
                friendlyMessage = !string.IsNullOrWhiteSpace(friendlyMessage) && !friendlyMessage.Equals(Success) ? friendlyMessage : "No actions required";
                internalMessage = !string.IsNullOrWhiteSpace(internalMessage) ? internalMessage : $"Procedure {functionName} did not require any changes.";
            }
            else if (resultType.Equals(DataResultType.Success))
            {
                // resultType = DataResultType.UnknownRecord;
                friendlyMessage = "Sorry, no results were returned.";
                internalMessage = $"Procedure {functionName} returned 0 records.";
            }

            var dataResult = new DataResult<TResult>(value, resultType, friendlyMessage, internalMessage, exception);
            dataResult.Data.Add("Command Details", functionName);

            if (valueId.HasValue)
            {
                dataResult.ValueId = valueId.Value;
            }

            exception?.Data.Add("ExceptionData.StringValue", functionName);

            UnitOfWork.AddDataResult(dataResult);

            return dataResult;
        }

        private string GetFormattedCallString(string functionName, List<string> parameters)
        {
            var parametersWithQuotes = parameters.Select(x => $"\"{x}\"");

            return $"CALL {functionName}({string.Join(",", parametersWithQuotes)})";
        }
    }
}