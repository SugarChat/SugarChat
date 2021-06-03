using Mediator.Net;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message;
using SugarChat.Message.Commands.Message;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services;
using SugarChat.Core.Basic;

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
                var message = new Core.Domain.Message
                {
                    GroupId = Guid.NewGuid().ToString(),
                    Content = "Test",
                    Type = MessageType.Text,
                    SentBy = Guid.NewGuid().ToString(),
                    Payload = "testUrl"
                };
                await repository.AddAsync(message);

                RevokeMessageCommand command = new RevokeMessageCommand
                {
                    MessageId = Guid.NewGuid().ToString()
                };

                {
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse<object>>(command);
                    response.Message.ShouldBe(string.Format(ServiceCheckExtensions.MessageExists, command.MessageId));
                }

                command.MessageId = message.Id;
                await mediator.SendAsync(command);
                (await repository.SingleOrDefaultAsync<Core.Domain.Message>(x => x.Id == command.MessageId)).IsRevoked.ShouldBeTrue();
            });
        }
    }
}