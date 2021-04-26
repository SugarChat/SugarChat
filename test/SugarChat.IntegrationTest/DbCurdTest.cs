using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Shouldly;
using SugarChat.Core.Common;
using SugarChat.Core.Domain;
using Xunit;

namespace SugarChat.IntegrationTest
{
    public class DbCurdTest : TestBase
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;

        public DbCurdTest()
        {
            var connectionString = _configuration.GetValue<string>("MongoDb:ConnectionString");
            var databaseName = _configuration.GetValue<string>("MongoDb:DatabaseName");

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }
        [Fact]
        public Task Should_Connect_To_Database()
        {
            _client.ShouldNotBe(null);
            _database.ShouldNotBe(null);
            
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Should_Add_New_Message()
        {
            var messages = _database.GetCollection<BaseMessage>("Message");
            Guid id = Guid.NewGuid();
            string content = "TestContent";
            
            await messages.InsertOneAsync(new BaseMessage(id, content, DateTime.Now, Guid.NewGuid(), Guid.NewGuid(),
                MessageStatus.Arrived, 1, null));
            var message = (await messages.FindAsync(o => o.Id == id)).FirstOrDefault();

            message.Id.ShouldBe(id);
            message.Content.ShouldBe(content);

            await messages.DeleteOneAsync(o => o.Id == id);
        }
        
        [Fact]
        public async Task Should_Delete_Message()
        {
            var messages = _database.GetCollection<BaseMessage>("Message");
            Guid id = Guid.NewGuid();
            
            await messages.InsertOneAsync(new BaseMessage(id, "", DateTime.Now, Guid.NewGuid(), Guid.NewGuid(),
                MessageStatus.Arrived, 1, null));
            var message = (await messages.FindAsync(o => o.Id == id)).FirstOrDefault();

            message.Id.ShouldBe(id);
            await messages.DeleteOneAsync(o => o.Id == id);
            message = (await messages.FindAsync(o => o.Id == id)).FirstOrDefault();
            message.ShouldBe(null);
        }
    }
}