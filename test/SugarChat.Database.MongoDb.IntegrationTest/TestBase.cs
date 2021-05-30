using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver;
using SugarChat.Core.IRepositories;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public abstract class TestBase : IAsyncDisposable, IDisposable
    {
        public IRepository Repository { get; }
        protected readonly MongoClient Client;
        protected readonly string DbName = MongoDbFactory.DbName;

        protected TestBase()
        {
            Repository = MongoDbFactory.GetRepository();
            Client = MongoDbFactory.GetClient();
        }

        public virtual ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public virtual void Dispose()
        {
        }
    }
}