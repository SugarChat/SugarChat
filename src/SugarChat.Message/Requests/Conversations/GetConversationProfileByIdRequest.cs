using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationProfileByIdRequest : IRequest
    {
        public string ConversationId{ get; set; }
    }
}
