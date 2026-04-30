using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using MongoDB.Driver;
using SugarChat.Core.Autofac;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Data.MongoDb.Autofac;
using SugarChat.Data.MongoDb.Settings;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    [Collection("DatabaseCollection")]
    public abstract class TestBase : IDisposable
    {
        private readonly DatabaseFixture _dbFixture;
        protected readonly IRepository Repository;
        protected readonly IConfiguration Configuration;
        protected readonly IMongoClient Client;
        protected readonly ILifetimeScope Container;

        protected TestBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            Configuration = _dbFixture.Configuration;
            Client = _dbFixture.Client;
            Container = _dbFixture.Container;
            Repository = _dbFixture.Repository;
        }

        public virtual void Dispose()
        {
            CleanDatabase();
        }

        private static readonly object databaseLock = new object();

        protected void CleanDatabase()
        {
            lock (databaseLock)
            {
                Client.DropDatabase(Configuration["MongoDb:DatabaseName"]);
            }
        }
    }
}