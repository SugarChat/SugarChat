using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SugarChat.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Message2 : Entity
    {
        public string Content { get; set; }
        public int Type { get; set; }
        public string SentBy { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public bool IsSystem { get; set; }
        public string Payload { get; set; }
        public bool IsRevoked { get; set; }
    }
}
