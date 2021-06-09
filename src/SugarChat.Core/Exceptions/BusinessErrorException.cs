﻿using System;
using Serilog.Events;

namespace SugarChat.Core.Exceptions
{
    public class BusinessErrorException : BusinessException
    {
        public BusinessErrorException(ExceptionPrompt prompt) : base(LogEventLevel.Error, prompt)
        {
        }
        public BusinessErrorException(ExceptionPrompt prompt, Exception innerException) : base(LogEventLevel.Error, prompt,
            innerException)
        {
        }
    }
}