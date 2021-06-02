using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Basic
{
    public interface ISugarChatResponse : IResponse
    {
        public int Code { get; set;  }

        public string Message { get; set; }
        
    }

    public interface ISugarChatResponse<T> : ISugarChatResponse
    {
        T Data { get; set; }
    }
    
}
