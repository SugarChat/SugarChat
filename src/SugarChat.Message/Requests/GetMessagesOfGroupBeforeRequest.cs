using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetMessagesOfGroupBeforeRequest : IRequest
    {
        public string MessageId { get; set; }
        public int Count { get; set; }
    }
}