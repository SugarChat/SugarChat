using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationListByUserIdRequest : IRequest
    {
        public string UserId { get; set; }
    }
}
