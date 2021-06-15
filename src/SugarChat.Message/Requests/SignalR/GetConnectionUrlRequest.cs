using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.SignalR
{
    public class GetConnectionUrlRequest : IRequest
    {
        public string UserId { get; set; }
    }
}