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
                    AttachmentUrl = "testUrl"
                };
                await mediator.SendAsync(command);
                (await repository.AnyAsync<Core.Domain.Message>(x => x.GroupId == command.GroupId
                    && x.Content == command.Content
                    && x.Type == command.Type
                    && x.SentBy == command.SentBy
                    && x.AttachmentUrl == command.AttachmentUrl)).ShouldBe(true);
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
                    AttachmentUrl = "testUrl"
                };
                await repository.AddAsync(message);

                RevokeMessageCommand command = new RevokeMessageCommand
                {
                    MessageId = Guid.NewGuid().ToString()
                };
                Func<Task> funcTask = () => mediator.SendAsync(command);
                funcTask.ShouldThrow(typeof(BusinessWarningException)).Message.ShouldBe(string.Format(ServiceCheckExtensions.MessageExists, command.MessageId));

                command.MessageId = message.Id;
                await mediator.SendAsync(command);
                (await repository.AnyAsync<Core.Domain.Message>(x => x.Id == command.MessageId && x.IsDel)).ShouldBe(true);
            });
        }
    }
}