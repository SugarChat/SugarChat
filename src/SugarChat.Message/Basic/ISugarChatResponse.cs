using Mediator.Net.Contracts;

namespace SugarChat.Message.Basic
{
    public interface ISugarChatResponse : IResponse
    {
        int Code { get; set;  }

        string Message { get; set; }
        
    }

    public interface ISugarChatResponse<T> : ISugarChatResponse
    {
        T Data { get; set; }
    }
    
}
