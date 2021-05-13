using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessWarningException : BusinessException
    {
        public BusinessWarningException(string message) : base(LogEventLevel.Warning, message)
        {
        }
        public BusinessWarningException(string message, Exception innerException) : base(LogEventLevel.Warning, message,
            innerException)
        {
        }
    }
}