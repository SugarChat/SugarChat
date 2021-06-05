using System;
using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {      
        public BusinessException(StatusCode code, string message, LogEventLevel logLevel = LogEventLevel.Error) : base(message)
        {
            LogLevel = logLevel;
            Code = code;
        }

        public LogEventLevel LogLevel { get; }

        public StatusCode Code { get; set; } = StatusCode.Ok;
    }
}