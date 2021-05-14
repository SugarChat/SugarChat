using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetAllHistoryToUserFromFriendRequest : IRequest
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
    }
}