using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetGroupsOfUserRequest : IRequest
    {
        public string Id { get; set; }
    }
}