using Mediator.Net;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message;
using SugarChat.Message.Commands.Message;
using SugarChat.Core.Services;
using SugarChat.Core.Basic;
using SugarChat.Core.Exceptions;

namespace SugarChat.IntegrationTest.Services
{
    public class MessageFixture : TestBase
    {
        [Fact]
        public async Task ShouldSendMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                SendMessageCommand command = new SendMessageCommand
                {
                    GroupId = Guid.NewGuid().ToString(),
                    Content = "Test",
                    Type = MessageType.Text,
                    SentBy = Guid.NewGuid().ToString(),
                    Payload = new
                    {
                        uuid = Guid.NewGuid(),
                        url = "testUrl",
                        size = 100,
                        second = 50
                    }
                };
                await mediator.SendAsync(command);
                (await repository.AnyAsync<Core.Domain.Message>(x => x.GroupId == command.GroupId
                    && x.Content == command.Content
                    && x.Type == command.Type
                    && x.SentBy == command.SentBy
                    && x.Payload == command.Payload)).ShouldBeTrue();
            });
        }

        [Fact]
        public async Task ShouldRevokeMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string messageId1 = Guid.NewGuid().ToString();
                string messageId2 = Guid.NewGuid().ToString();
                var message = new Core.Domain.Message
                {
                    Id = messageId1,
                    GroupId = Guid.NewGuid().ToString(),
                    Content = "Test",
                    Type = MessageType.Text,
                    SentBy = Guid.NewGuid().ToString(),
                    SentTime = DateTime.Now.AddMinutes(-5),
                    Payload = "testUrl"
                };
                await repository.AddAsync(message);
                message.Id = messageId2;
                message.SentTime = DateTime.Now;
                await repository.AddAsync(message);

                RevokeMessageCommand command = new RevokeMessageCommand
                {
                    MessageId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString()
                };
                {
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(string.Format(ExceptionPrompt.MessageExists, command.MessageId));
                }
                {
                    command.MessageId = messageId1;
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe("no authorization");
                }
                {
                    command.UserId = message.SentBy;
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe("the sending time is more than two minutes and cannot be withdrawn");
                }

                command.MessageId = messageId2;
                await mediator.SendAsync(command);
                (await repository.SingleOrDefaultAsync<Core.Domain.Message>(x => x.Id == command.MessageId)).IsRevoked.ShouldBeTrue();
            });
        }
    }
}