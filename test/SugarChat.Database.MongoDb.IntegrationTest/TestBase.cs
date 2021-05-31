using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Settings;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    [Collection("DatabaseCollection")]
    public abstract class TestBase : IAsyncDisposable, IDisposable
    {
        private readonly DatabaseFixture _dbFixture;
        protected readonly IRepository Repository;
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected TestBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            var settings = _dbFixture.Settings;
            settings.DatabaseName = _dbName;
            Repository = new MongoDbRepository(settings);
        }
       
        public virtual async ValueTask DisposeAsync()
        {
            await _dbFixture.Client.DropDatabaseAsync(_dbName);
        }

        public virtual void Dispose()
        {
            _dbFixture.Client.DropDatabase(_dbName);
        }
    }
}