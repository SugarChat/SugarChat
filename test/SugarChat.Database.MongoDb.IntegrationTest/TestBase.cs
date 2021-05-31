using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Settings;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public abstract class TestBase : IAsyncDisposable, IDisposable
    {
        protected readonly IRepository Repository;
        private readonly MongoClient _client;
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected TestBase()
        {
            var settings = new MongoDbSettings();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            configuration.GetSection("MongoDb")
                .Bind(settings);
            settings.DatabaseName = _dbName;
            
            Repository = new MongoDbRepository(settings);
            _client = new MongoClient(settings.ConnectionString);
        }
       
        public virtual async ValueTask DisposeAsync()
        {
            await _client.DropDatabaseAsync(_dbName);
        }

        public virtual void Dispose()
        {
            _client.DropDatabase(_dbName);
        }
    }
}