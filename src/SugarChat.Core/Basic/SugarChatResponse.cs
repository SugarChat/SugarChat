using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Basic
{
    public class SugarChatResponse<T> : ISugarChatResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class SugarChatResponse : ISugarChatResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}