using Mediator.Net.Contracts;
using SugarChat.Message.Commands;

namespace SugarChat.Message.Requests
{
    public class GetUnreadMessagesFromGroupRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public string MessageId { get; set; }
        public int Count { get; set; } = 15;
    }
}