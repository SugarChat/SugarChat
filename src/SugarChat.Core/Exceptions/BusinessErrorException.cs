using System;
using Serilog.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Exceptions
{
    public class BusinessErrorException : BusinessException
    {
        public BusinessErrorException(StatusCode code, string message) : base(LogEventLevel.Error, code, message)
        {
        }

        public BusinessErrorException(StatusCode code, string message, Exception innerException) : base(LogEventLevel.Error, code, message,
            innerException)
        {
        }
    }
}