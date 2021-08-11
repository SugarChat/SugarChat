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
        public async Task ShouldSetMessageReadByUserIdsBasedOnGroupId()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                string groupId = Guid.NewGuid().ToString();
                await repository.AddAsync(new Group
                {
                    Id = groupId
                });

                DateTimeOffset lastMessageSentTime = DateTime.Now.AddDays(-1);
                List<GroupUser> groupUsers = new List<GroupUser>();
                await repository.AddAsync(new Core.Domain.Message
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    SentTime = lastMessageSentTime
                });

                for (int i = 0; i < 10; i++)
                {
                    groupUsers.Add(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = groupId,
                        UserId = Guid.NewGuid().ToString()
                    });
                }
                await repository.AddRangeAsync(groupUsers);
                IEnumerable<string> userIds = groupUsers.Select(x => x.UserId);

                SetMessageReadByUserIdsBasedOnGroupIdCommand command = new SetMessageReadByUserIdsBasedOnGroupIdCommand()
                {
                    GroupId = Guid.NewGuid().ToString(),
                    UserIds = new string[] { Guid.NewGuid().ToString() }
                };
                {
                    var response = await mediator.SendAsync<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.GroupNoExists.WithParams(command.GroupId).Message);
                }
                {
                    command.GroupId = groupId;
                    var response = await mediator.SendAsync<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotAllUsersExists.Message);
                }
                {
                    command.UserIds = userIds;
                    await mediator.SendAsync(command);
                    (await repository.CountAsync<GroupUser>(x => x.GroupId == groupId && userIds.Contains(x.UserId) && x.LastReadTime == lastMessageSentTime)).ShouldBe(10);
                }
            });
        }
    }
}