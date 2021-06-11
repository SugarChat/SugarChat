using Mediator.Net.Contracts;

namespace SugarChat.Message.Responses.Messages
{
    public class GetUnreadMessageCountResponse : IResponse
    {
        public int Count { get; set; }
    }
}
