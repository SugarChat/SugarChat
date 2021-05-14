using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetUnreadToUserFromGroupRequest : IRequest
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}