using Mediator.Net.Contracts;
using System.Collections.Generic;

namespace SugarChat.Message.Requests.Messages
{
    public class GetUnreadMessageCountRequest : IRequest
    {
        public string UserId { get; set; }
        public IEnumerable<string> GroupIds { get; set; } = new List<string>();
    }
}
