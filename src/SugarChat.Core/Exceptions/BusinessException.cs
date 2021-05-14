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
        public BusinessException(LogEventLevel logLevel, int code, string message) : base(message)
        {
            LogLevel = logLevel;
            Code = code;
        }
        
        public BusinessException(LogEventLevel logLevel, int code, string message, Exception innerException) : base(message, innerException)
        {
            LogLevel = logLevel;
            Code = code;
        }

        public LogEventLevel LogLevel { get; }

        public int Code { get; set; } = 0;
    }
}