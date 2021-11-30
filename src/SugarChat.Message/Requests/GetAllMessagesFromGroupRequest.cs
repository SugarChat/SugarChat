using Mediator.Net.Contracts;
using SugarChat.Message.Commands;

namespace SugarChat.Message.Requests
{
    public class GetAllMessagesFromGroupRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public int Index { get; set; }
        public string MessageId { get; set; }
        public int Count { get; set; } = 15;
    }
}