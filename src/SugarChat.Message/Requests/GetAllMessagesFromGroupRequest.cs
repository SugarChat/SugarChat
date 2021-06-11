using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetAllMessagesFromGroupRequest : IRequest
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string MessageId { get; set; }
        public int Count { get; set; } = 15;
    }
}