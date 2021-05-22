using System;

namespace SugarChat.Core.Domain
{
    public class MessageUnread : Entity
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
    }
}