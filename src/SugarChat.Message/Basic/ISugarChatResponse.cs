using Mediator.Net.Contracts;

namespace SugarChat.Message.Basic
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
