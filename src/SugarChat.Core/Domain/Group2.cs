using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Group2 : Entity
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public DateTimeOffset LastSentTime { get; set; }
        public List<GroupUser2> GroupUsers { get; set; } = new List<GroupUser2>();
        public List<Message2> Messages { get; set; } = new List<Message2>();
    }
}
