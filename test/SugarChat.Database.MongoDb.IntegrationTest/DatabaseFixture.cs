using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Settings;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class DatabaseFixture : IDisposable
    {
        public MongoClient Client{ get;}
        public MongoDbSettings Settings { get;}
        public DatabaseFixture()
        {
            Settings = new MongoDbSettings();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            configuration.GetSection("MongoDb")
                .Bind(Settings);
            Client = new MongoClient(Settings.ConnectionString);
        }

        public void Dispose()
        {
        }
    }
}