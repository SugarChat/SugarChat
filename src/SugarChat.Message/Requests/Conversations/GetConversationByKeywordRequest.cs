using Mediator.Net.Contracts;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Requests.Conversations
{
    public class GetConversationByKeywordRequest : IRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, string> SearchParms { get; set; }
        public PageSettings PageSettings { get; set; }
        public bool IsExactSearch { get; set; }
    }
}
