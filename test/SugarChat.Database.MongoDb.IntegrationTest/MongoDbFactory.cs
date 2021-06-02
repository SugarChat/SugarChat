using Microsoft.Extensions.Configuration;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Settings;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public static class MongoDbFactory
    {
        public static IRepository GetRepository()
        {
            MongoDbSettings settings = new MongoDbSettings();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            configuration.GetSection("MongoDb")
                .Bind(settings);
            return new MongoDbRepository(settings);
        }
    }
}