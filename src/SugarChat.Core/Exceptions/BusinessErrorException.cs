using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessErrorException : BusinessException
    {
        public BusinessErrorException(int code, string message) : base(LogEventLevel.Error, code, message)
        {
        }

        public BusinessErrorException(int code, string message, Exception innerException) : base(LogEventLevel.Error, code, message,
            innerException)
        {
        }
    }
}