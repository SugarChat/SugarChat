using Microsoft.Extensions.Configuration;
using Shouldly;
using SugarChat.Core.Common;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Settings;
using SugarChat.Infrastructure.Contexts;
using SugarChat.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest
{
    public class MessageRepositoryTest : TestBase
    {
        readonly IMessageRepository msgRepository;
        public MessageRepositoryTest()
        {
            MongoDbSettings settings = new MongoDbSettings();
            _configuration.GetSection("MongoDb").Bind(settings);
            msgRepository = new MessageRepository(new SugarChatDbContext(settings));
        }

        [Fact]
        public Task Should_Message_Repo_Not_Null()
        {
            msgRepository.ShouldNotBeNull();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Should_Add_One_Message()
        {
            var id = Guid.NewGuid();
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var newMsg = new Message(id, "message test!", DateTime.Now, fromId, toId, MessageStatus.Sent);
            await msgRepository.AddAsync(newMsg);
            var insertedMsg = await msgRepository.FindAsync(id);
            insertedMsg.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Delete_One_Message()
        {
            var id = Guid.NewGuid();
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var newMsg = new Message(id, "message test!", DateTime.Now, fromId, toId, MessageStatus.Sent);
            await msgRepository.AddAsync(newMsg);
            var insertedMsg = await msgRepository.FindAsync(id);
            insertedMsg.ShouldNotBeNull();
            await msgRepository.DeleteAsync(insertedMsg.Id);
            var deletedMsg = await msgRepository.FindAsync(id);
            deletedMsg.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Update_One_Message()
        {
            var id = Guid.NewGuid();
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();
            var newMsg = new Message(id, "message test!", DateTime.Now, fromId, toId, MessageStatus.Sent);
            await msgRepository.AddAsync(newMsg);
            var insertedMsg = await msgRepository.FindAsync(id);
            insertedMsg.ShouldNotBeNull();
            string newMsgContent = "new message test!";
            DateTime newPublishDate = DateTime.Now;
            Guid newToId = Guid.NewGuid();
            var newUpdateMsg = new Message(insertedMsg.Id,
                newMsgContent,
                newPublishDate,
                insertedMsg.From,
                newToId,
                insertedMsg.Status
                );
            await msgRepository.UpdateAsync(newUpdateMsg, entity => new
            {
                entity.Content,
                entity.PublishDateTime
            });
            var updatedMsg = await msgRepository.FindAsync(id);
            updatedMsg.Content.ShouldBe(newMsgContent);
            Assert.Equal(updatedMsg.PublishDateTime.ToString("yyyyMMdd HH:mm:ss"), newPublishDate.ToString("yyyyMMdd HH:mm:ss"));
            updatedMsg.From.ShouldBe(insertedMsg.From);
            updatedMsg.To.ShouldBe(insertedMsg.To);
            updatedMsg.Status.ShouldBe(insertedMsg.Status);
        }

    }
}
