using MongoDB.Bson.Serialization;

namespace SugarChat.Database.MongoDb.Mapping
{
    public class MessageMap
    {
        public MessageMap()
        {
            BsonClassMap.RegisterClassMap<Core.Domain.Message>(cm => {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            });
        }
    }
}
