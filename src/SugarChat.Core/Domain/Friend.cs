using System;

namespace SugarChat.Core.Domain
{
    public class Friend : Entity
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public DateTimeOffset BecomeFriendAt { get; set; }
    }
}
