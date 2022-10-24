using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    [BsonIgnoreExtraElements]
    public class Group : Entity
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        [BsonIgnore]
        public IEnumerable<GroupCustomProperty> CustomPropertyList { get; set; }
    }
}
