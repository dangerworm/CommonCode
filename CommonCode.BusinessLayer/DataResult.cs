using System;
using System.Collections.Generic;
using CommonCode.BusinessLayer.Helpers;

namespace CommonCode.BusinessLayer
{
    public class DataResult
    {
        private string _friendlyMessage;

        public IDictionary<string, object> Data { get; private set; }
        public ValidationCollection Validation { get; private set; }

        public DataResultType Type { get; set; }
        public Exception Exception { get; set; }
        public string InternalMessage { get; set; }

        public string FriendlyMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_friendlyMessage) && Type != DataResultType.Success)
                {
                    return "An error has occurred; please try again. If the problem persists please contact the developer.";
                }
                return _friendlyMessage;
            }
            set
            {
                _friendlyMessage = value;
            }
        }

        public DataResult(DataResultType type, string friendlyMessage, string internalMessage)
        {
            Verify.ValidEnumValue(type, nameof(type));
            Verify.ValidString(friendlyMessage, nameof(friendlyMessage));
            Verify.ValidString(internalMessage, nameof(internalMessage));

            Type = type;
            FriendlyMessage = friendlyMessage;
            InternalMessage = internalMessage;

            Validation = new ValidationCollection();
            Data = new Dictionary<string, object>();
        }

        public DataResult(DataResultType type, string message)
            : this(type, message, message)
        {
        }

        public DataResult(DataResultType type, string message, Exception exception)
            : this(type, message)
        {
            Exception = exception;
        }

        public DataResult(DataResultType type, string friendlyMessage, string internalMessage, Exception exception)
            : this(type, friendlyMessage, internalMessage)
        {
            Exception = exception;
        }
    }
}