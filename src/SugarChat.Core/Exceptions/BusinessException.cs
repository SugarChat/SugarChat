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
        public BusinessException(LogEventLevel logLevel, ExceptionPrompt prompt) : base(prompt.Message)
        {
            LogLevel = logLevel;
            Code = prompt.Code;
        }
        
        public BusinessException(LogEventLevel logLevel, ExceptionPrompt prompt, Exception innerException) : base(prompt.Message, innerException)
        {
            LogLevel = logLevel;
            Code = prompt.Code;
        }

        public LogEventLevel LogLevel { get; }

        public int Code { get; }
    }
}