using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SugarChat.Core.Settings;

namespace SugarChat.Infrastructure.Contexts
{
    public abstract class Context<T>
    {
        private readonly IMongoDatabase _database = null;
        public Context(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        protected virtual IMongoDatabase Database => _database;
        public abstract IMongoCollection<T> Collection { get; }
    }
}
