using MongoDB.Bson.Serialization.Attributes;
using SugarChat.Message;
using System;

namespace SugarChat.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class GroupUser : Entity
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public DateTimeOffset? LastReadTime { get; set; }
        public UserRole Role { get; set; }
        public MessageRemindType MessageRemindType { get; set; }
        public int UnreadCount { get; set; }
    }
}
