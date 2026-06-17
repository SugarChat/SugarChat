using MongoDB.Bson.Serialization.Attributes;
using SugarChat.Message;
using System;

namespace SugarChat.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class GroupUser2 : Entity
    {
        public string UserId { get; set; }
        public DateTimeOffset? LastReadTime { get; set; }
        public UserRole Role { get; set; }
        public MessageRemindType MessageRemindType { get; set; }
        public int UnreadCount { get; set; }
        public DateTimeOffset LastSentTime { get; set; }
    }
}
