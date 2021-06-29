using Mediator.Net.Contracts;
using SugarChat.Shared.Paging;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationListRequest : IRequest
    {     
        public string UserId { get; set; }
        public PageSettings PageSettings { get; set; }
    }
}
