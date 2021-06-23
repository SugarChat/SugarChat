using Mediator.Net.Contracts;
using SugarChat.Shared.Paging;

namespace SugarChat.Message.Requests
{
    public class GetGroupIdsOfUserRequest : IRequest
    {
        public string Id { get; set; }
    }
}