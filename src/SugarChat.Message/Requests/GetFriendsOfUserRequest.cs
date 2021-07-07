using Mediator.Net.Contracts;
using SugarChat.Message.Paging;

namespace SugarChat.Message.Requests
{
    public class GetFriendsOfUserRequest : IRequest
    {
        public string Id { get; set; }
        public PageSettings PageSettings { get; set; }
    }
}