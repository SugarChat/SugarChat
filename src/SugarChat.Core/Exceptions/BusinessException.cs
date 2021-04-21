using System;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {
        public BusinessException(string message) : base(message)
        {
            
        }
    }
}