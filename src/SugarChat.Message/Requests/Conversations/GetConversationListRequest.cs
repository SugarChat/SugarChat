using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationListRequest : IRequest
    {     
        public string UserId { get; set; }
    }
}
