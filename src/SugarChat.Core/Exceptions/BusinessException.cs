using System;
using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {

        public BusinessException(string message, LogEventLevel logLevel = LogEventLevel.Error) : base(message)
        {
            LogLevel = logLevel;
        }
        public BusinessException(StatusCode code, string message, LogEventLevel logLevel = LogEventLevel.Error) : base(message)
        {
            LogLevel = logLevel;
            Code = code;
        }

        public BusinessException(StatusCode code, string message, Exception innerException, LogEventLevel logLevel = LogEventLevel.Error) : base(message, innerException)
        {
            LogLevel = logLevel;
            Code = code;
        }

        public LogEventLevel LogLevel { get; }

        public StatusCode Code { get; set; } = 0;
    }
}