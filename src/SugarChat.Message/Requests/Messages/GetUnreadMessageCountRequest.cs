using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Messages
{
    public class GetUnreadMessageCountRequest : IRequest
    {
        public string UserId { get; set; }
    }
}
