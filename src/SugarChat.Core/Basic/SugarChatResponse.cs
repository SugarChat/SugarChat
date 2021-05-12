using System;
using System.Collections.Generic;
using System.Text;

namespace SugarChat.Core.Basic
{
    public class SugarChatResponse : ISugarChatResponse
    {
        public SugarChatResponse()
        {
        }

        public SugarChatResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; } = 0;

        public string Message { get; set; } = "success";
        public object Data { get; set; }

        public static SugarChatResponse CreateResponse()
        {
            return new SugarChatResponse();
        }
        public static SugarChatResponse CreateResponse(int code, string message)
        {
            return new SugarChatResponse(code, message);
        }
    }
    public class SugarResponse<T> : SugarChatResponse
    {
        public SugarResponse()
        {
        }

        public SugarResponse(T data)
        {
            Data = data;
        }

        public SugarResponse(int code, string message, T data) : base(code, message)
        {
            Data = data;
        }

        public new T Data { get; set; } = default;

        public static SugarResponse<T> CreateResponse(int code, string message, T data)
        {
            return new SugarResponse<T>(code, message, data);
        }
    }
}
