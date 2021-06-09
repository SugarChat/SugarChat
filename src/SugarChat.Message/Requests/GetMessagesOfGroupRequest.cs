using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetMessagesOfGroupRequest : IRequest
    {
        public string GroupId { get; set; }
        public int Count { get; set; }
    }
}