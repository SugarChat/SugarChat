using Mediator.Net.Contracts;

namespace SugarChat.Message.Responses.SignalR
{
    public class GetConnectionUrlResponse : IResponse
    {
        public string ConnectionUrl { get; set; }   
    }
}