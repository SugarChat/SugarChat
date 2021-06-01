using SugarChat.Message;
using System;

namespace SugarChat.Core.Domain
{
    public class GroupUser : Entity
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public DateTimeOffset? LastReadTime { get; set; }
        public bool IsMaster { get; set; }
        public bool IsAdmin { get; set; }
        public MessageRemindType MessageRemindType { get; set; }
    }
}
