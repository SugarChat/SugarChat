using SugarChat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Domain
{
    public class GroupUser2 : Entity
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public DateTimeOffset? LastReadTime { get; set; }
        public UserRole Role { get; set; }
        public MessageRemindType MessageRemindType { get; set; }
        public int UnreadCount { get; set; }
        public int GroupType { get; set; }
        public DateTimeOffset LastSentTime { get; set; }
    }
}
