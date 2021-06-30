using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Requests.Messages
{
    public class GetMessagesByGroupIdsRequest : IRequest
    {
        public string UserId { get; set; }

        public IEnumerable<string> GroupIds { get; set; }
    }
}
