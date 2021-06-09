using Mediator.Net.Contracts;

namespace SugarChat.Message.Requests
{
    public class GetGroupMembersRequest : IRequest
    {
        public string GroupId { get; set; }
    }
}