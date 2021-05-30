using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Settings;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public static class MongoDbFactory
    {
        private static readonly MongoDbSettings _settings;
        public static readonly string DbName ;

        static MongoDbFactory()
        {
            _settings = new MongoDbSettings();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            configuration.GetSection("MongoDb")
                .Bind(_settings);
            DbName = configuration["MongoDb:DatabaseName"];
        }
        public static IRepository GetRepository()
        {
            return new MongoDbRepository(_settings);
        }
        
        public static MongoClient GetClient()
        {
            return new(_settings.ConnectionString);
        }

    }
}