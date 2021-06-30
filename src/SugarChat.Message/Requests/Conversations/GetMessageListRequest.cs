using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetMessageListRequest : IRequest
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public string NextReqMessageId { get; set; }
        public int Count { get; set; }
        public int PagaIndex { get; set; }
    }
}
