using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationProfileRequest : IRequest
    {
        public string ConversationId{ get; set; }
        public string UserId { get; set; }
    }
}
