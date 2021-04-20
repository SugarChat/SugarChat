using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.Settings;

namespace SugarChat.Infrastructure.Contexts
{
    public class UserContext : Context<User>
    {
        public UserContext(IOptions<MongoDbSettings> options) : base(options)
        {
        }
        public override IMongoCollection<User> Collection
        {
            get
            {
                return Database.GetCollection<User>("User");
            }
        }
    }
}
