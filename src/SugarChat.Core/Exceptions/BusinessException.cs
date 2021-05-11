using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {
        public BusinessException(LogEventLevel logLevel, string message) : base(message)
        {
            LogLevel = logLevel;
        }
        
        public BusinessException(LogEventLevel logLevel, string message, Exception innerException) : base(message, innerException)
        {
            LogLevel = logLevel;
        }

        public LogEventLevel LogLevel { get; }
    }
}