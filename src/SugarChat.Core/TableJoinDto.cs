using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core
{
    public class TableJoinDto
    {
        public string GroupId { get; set; }
        public int UnreadCount { get; set; }
        public DateTimeOffset LastSentTime { get; set; }
        public string GroupKey { get; set; }
        public string GroupValue { get; set; }
        public string Content { get; set; }
        public string MessageKey { get; set; }
        public string MessageValue { get; set; }
        public int Count { get; set; }
    }
}
