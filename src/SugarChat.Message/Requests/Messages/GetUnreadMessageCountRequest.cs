using Mediator.Net.Contracts;
using SugarChat.Message.Commands;
using System.Collections.Generic;

namespace SugarChat.Message.Requests.Messages
{
    public class GetUnreadMessageCountRequest : IRequest, INeedUserExist
    {
        public string UserId { get; set; }
        public IEnumerable<string> GroupIds { get; set; } = new List<string>();
    }
}
