using System;
using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {

        public BusinessException(LogEventLevel logLevel, string message) : base(message)
        {
            LogLevel = logLevel;
        }
        public BusinessException(LogEventLevel logLevel, StatusCode code, string message) : base(message)
        {
            LogLevel = logLevel;
            Code = code;
        }
        
        public BusinessException(LogEventLevel logLevel, StatusCode code, string message, Exception innerException) : base(message, innerException)
        {
            LogLevel = logLevel;
            Code = code;
        }

        public LogEventLevel LogLevel { get; }

        public StatusCode Code { get; set; } = 0;
    }
}