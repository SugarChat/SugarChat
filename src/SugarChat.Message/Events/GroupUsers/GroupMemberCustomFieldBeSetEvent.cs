using System.Collections.Generic;

namespace SugarChat.Message.Events.GroupUsers
{
    public class GroupMemberCustomFieldBeSetEvent : EventBase
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
    }
}
