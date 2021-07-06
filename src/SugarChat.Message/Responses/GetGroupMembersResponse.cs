using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Responses
{
    public class GetGroupMembersResponse : IResponse
    {
        public IEnumerable<string> MemberIds { get; set; }
    }
}