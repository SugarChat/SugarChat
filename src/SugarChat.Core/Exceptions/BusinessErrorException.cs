using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessErrorException : BusinessException
    {
        public BusinessErrorException(string message) : base(LogEventLevel.Error, message)
        {
        }

        public BusinessErrorException(string message, Exception innerException) : base(LogEventLevel.Error, message,
            innerException)
        {
        }
    }
}