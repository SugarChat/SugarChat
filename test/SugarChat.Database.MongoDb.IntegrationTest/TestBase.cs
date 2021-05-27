using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public abstract class TestBase : IAsyncDisposable
    {
        protected readonly IConfiguration _configuration;

        protected TestBase()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public virtual ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}