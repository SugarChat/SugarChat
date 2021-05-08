using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Core.Basic
{
    public class BasicResponse
    {
        public BasicResponse()
        {
        }

        public BasicResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; } = 0;

        public string Message { get; set; } = "success";
    }
    public class BasicResponse<T> : BasicResponse
    {
        public BasicResponse()
        {
        }

        public BasicResponse(T data)
        {
            Data = data;
        }

        public BasicResponse(int code, string message, T data) : base(code, message)
        {
            Data = data;
        }

        public T Data { get; set; } = default;
    }
}
