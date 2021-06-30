using Mediator.Net.Contracts;
using System.Collections.Generic;

namespace SugarChat.Message.Responses.GroupUsers
{
    public class GetUserIdsByGroupIdsResponse : IResponse
    {
        public IEnumerable<string> UserIds { get; set; }
    }
}
