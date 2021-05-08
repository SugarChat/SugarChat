using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetUserRequest : IRequest
    {
        public string Id { get; set; }
    }
}