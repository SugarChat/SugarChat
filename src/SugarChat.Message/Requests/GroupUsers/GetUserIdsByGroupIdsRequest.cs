using Mediator.Net.Contracts;
using System.Collections.Generic;

namespace SugarChat.Message.Requests.GroupUsers
{
    public class GetUserIdsByGroupIdsRequest : IRequest
    {
        public IEnumerable<string> GroupIds { get; set; }
    }
}
