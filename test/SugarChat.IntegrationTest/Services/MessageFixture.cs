using Mediator.Net;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message;
using SugarChat.Core.Services;
using SugarChat.Core.Basic;
using SugarChat.Core.Exceptions;
using SugarChat.Message.Commands.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Messages;
using System.Linq;

namespace SugarChat.IntegrationTest.Services
{
    public class MessageFixture : TestBase
    {
        [Fact]
        public async Task ShouldSendMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                object payload = new
                {
                    uuid = Guid.NewGuid(),
                    url = "testUrl",
                    size = 100,
                    second = 50
                };
                SendMessageCommand command = new SendMessageCommand
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = Guid.NewGuid().ToString(),
                    Content = "Test",
                    Type = 0,
                    SentBy = Guid.NewGuid().ToString(),
                    Payload = JsonConvert.SerializeObject(payload),
                    CreatedBy = Guid.NewGuid().ToString(),
                    CustomProperties = new Dictionary<string, string> { { "Number", "1" } }
                };
                await mediator.SendAsync(command);
                var message = await repository.SingleAsync<Core.Domain.Message>(x => x.GroupId == command.GroupId
                     && x.Content == command.Content
                     && x.Type == command.Type
                     && x.SentBy == command.SentBy
                     && x.Payload == command.Payload
                     && x.CreatedBy == command.CreatedBy
                     && x.CustomProperties == command.CustomProperties);
                message.CustomProperties.GetValueOrDefault("Number").ShouldBe("1");
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
                    Type = 0,
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
                    response.Message.ShouldBe(Prompt.MessageNoExists.WithParams(command.MessageId).Message);
                }
                {
                    command.MessageId = messageId1;
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.RevokeOthersMessage.WithParams(command.UserId, command.MessageId).Message);
                }
                {
                    command.UserId = message.SentBy;
                    var response = await mediator.SendAsync<RevokeMessageCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.TooLateToRevoke.WithParams(command.UserId, command.MessageId).Message);
                }

                command.MessageId = messageId2;
                await mediator.SendAsync(command);
                (await repository.SingleOrDefaultAsync<Core.Domain.Message>(x => x.Id == command.MessageId)).IsRevoked.ShouldBeTrue();
            });
        }

        [Fact]
        public async Task ShouldGetLastMessageForGroups()
        {
            await Run<IMessageDataProvider, IRepository>(async (messageDataProvider, repository) =>
            {
                Guid[] groupIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
                for (int i = 0; i < groupIds.Length; i++)
                {
                    await repository.AddAsync(new Group
                    {
                        Id = groupIds[i].ToString(),
                        Name = "group" + i
                    });
                    for (int j = 0; j < 3; j++)
                    {
                        DateTimeOffset sentTime = DateTimeOffset.Now.AddMinutes(j);
                        await repository.AddAsync(new Core.Domain.Message
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId= groupIds[i].ToString(),
                            Content = "Content" + i + j,
                            SentTime = DateTimeOffset.Now
                        });
                    }
                    for (int j = 4; j < 6; j++)
                    {
                        DateTimeOffset sentTime = DateTimeOffset.Now.AddMinutes(-j);
                        await repository.AddAsync(new Core.Domain.Message
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = groupIds[i].ToString(),
                            Content = "Content" + i + j,
                            SentTime = DateTimeOffset.Now
                        });
                    }
                }
                var messages = await messageDataProvider.GetLastMessageForGroupsAsync(groupIds.Select(x => x.ToString()));
                messages.Count().ShouldBe(5);
                for (int i = 0; i < groupIds.Length; i++)
                {
                    var message = messages.Single(x => x.GroupId == groupIds[i].ToString());
                    message.Content.ShouldBe("Content" + i + "5");
                }
            });
        }
    }
}