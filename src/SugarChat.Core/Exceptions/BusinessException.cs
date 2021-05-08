using System;

namespace SugarChat.Core.Exceptions
{
    public class BusinessException : Exception, IBusinessException
    {
        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(int code, string message) : base(message)
        {
            Code = code;
        }

        public int Code { get; set; } = 0;
    }
}