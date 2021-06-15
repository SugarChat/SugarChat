using Mediator.Net.Contracts;
using SugarChat.Shared.Paging;

namespace SugarChat.Message.Requests
{
    public class GetPagedGroupsOfUserRequest : IRequest
    {
        public string Id { get; set; }
        public PageSettings PageSettings { get; set; }
        
    }
}