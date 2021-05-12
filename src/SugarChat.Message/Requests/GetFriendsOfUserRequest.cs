using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetFriendsOfUserRequest : IRequest
    {
        public string Id { get; set; }
    }
}