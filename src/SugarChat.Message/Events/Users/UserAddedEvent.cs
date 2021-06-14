using System.Collections.Generic;

namespace SugarChat.Message.Events.Users
{
    public class UserAddedEvent : EventBase
    {
        public string Id { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
    }
}