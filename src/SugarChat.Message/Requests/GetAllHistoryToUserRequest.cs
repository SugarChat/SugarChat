using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetAllHistoryToUserRequest : IRequest
    {
        public string UserId { get; set; }
    }
}