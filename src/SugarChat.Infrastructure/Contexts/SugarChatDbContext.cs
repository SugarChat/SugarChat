using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.Settings;

namespace SugarChat.Infrastructure.Contexts
{
    public class SugarChatDbContext
    {
        private readonly IMongoDatabase _database = null;
        public SugarChatDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.DatabaseName);
        }
        protected virtual IMongoDatabase Database => _database;

        public IMongoCollection<User> User => Database.GetCollection<User>(nameof(User));

        public IMongoCollection<Message> Message => Database.GetCollection<Message>(nameof(Message));
    }
}
