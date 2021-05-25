using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetAllUnreadToUserRequest : IRequest
    {
        public string UserId { get; set; }
    }
}