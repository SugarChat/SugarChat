using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Basic
{
    public class SugarChatResponse : ISugarChatResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class SugarChatResponse<T> : SugarChatResponse, ISugarChatResponse<T>
    {
        public T Data { get; set; }
    }
}
