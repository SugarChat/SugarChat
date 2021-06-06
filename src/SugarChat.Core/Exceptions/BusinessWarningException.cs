﻿using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessWarningException : BusinessException
    {
        public BusinessWarningException(string message) : base(LogEventLevel.Warning, message)
        {
        }
        public BusinessWarningException(ExceptionPrompt prompt) : base(LogEventLevel.Warning, prompt)
        {
        }
        public BusinessWarningException(ExceptionPrompt prompt, Exception innerException) : base(LogEventLevel.Warning, prompt,
            innerException)
        {
        }
    }
}