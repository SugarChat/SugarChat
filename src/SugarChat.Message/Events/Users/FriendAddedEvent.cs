using System;

namespace SugarChat.Message.Events.Users
{
    public class FriendAddedEvent : EventBase
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public DateTimeOffset BecomeFriendAt { get; set; }
    }
}