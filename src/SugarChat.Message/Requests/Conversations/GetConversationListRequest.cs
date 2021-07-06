using Mediator.Net.Contracts;
using SugarChat.Message.Paging;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationListRequest : IRequest
    {     
        public string UserId { get; set; }
        public PageSettings PageSettings { get; set; }
    }
}
