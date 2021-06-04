using System;
using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public class BusinessWarningException : BusinessException
    {
        public BusinessWarningException(string message) : base(LogEventLevel.Warning, message)
        {
        }
        public BusinessWarningException(StatusCode code, string message) : base(LogEventLevel.Warning, code, message)
        {
        }
        public BusinessWarningException(StatusCode code, string message, Exception innerException) : base(LogEventLevel.Warning, code, message,
            innerException)
        {
        }
    }
}