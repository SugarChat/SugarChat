using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetMessageListByPageIndexRequest : IRequest
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public int PagaIndex { get; set; }
        public int Count { get; set; }
    }
}
