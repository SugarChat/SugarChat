using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    public class Group : Entity
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Description { get; set; }
        [BsonIgnore]
        public IEnumerable<GroupCustomProperty> CustomProperties { get; set; }
    }
}
