using Mediator.Net;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using SugarChat.Message;
using SugarChat.Core.Services;
using SugarChat.Message.Exceptions;
using SugarChat.Message.Commands.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Messages;
using System.Linq;
using SugarChat.Message.Basic;
using AutoMapper;
using SugarChat.Message.Dtos;

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
                            GroupId = groupIds[i].ToString(),
                            Content = "Content" + i + j,
                            SentTime = sentTime
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
                            SentTime = sentTime
                        });
                    }
                }
                var messages = await messageDataProvider.GetLastMessageForGroupsAsync(groupIds.Select(x => x.ToString()));
                messages.Count().ShouldBe(5);
                for (int i = 0; i < groupIds.Length; i++)
                {
                    var message = messages.Single(x => x.GroupId == groupIds[i].ToString());
                    message.Content.ShouldBe("Content" + i + "2");
                }
            });
        }

        [Fact]
        public async Task ShouldUpdateMessageData()
        {
            await Run<IMediator, IRepository, IMapper>(async (mediator, repository, mapper) =>
            {
                var userId1 = Guid.NewGuid().ToString();
                var userId2 = Guid.NewGuid().ToString();
                var groupId1= Guid.NewGuid().ToString();
                var groupId2 = Guid.NewGuid().ToString();
                await repository.AddAsync(new Group
                {
                    Id = groupId1,
                    Name = "testGroup1",
                    AvatarUrl = "testAvatarUrl1",
                    Description = "testDescription1"
                });
                await repository.AddAsync(new Group
                {
                    Id = groupId2,
                    Name = "testGroup2",
                    AvatarUrl = "testAvatarUrl2",
                    Description = "testDescription2"
                });
                await repository.AddAsync(new User
                {
                    Id = userId1
                });
                await repository.AddAsync(new User
                {
                    Id = userId2
                });
                List<SendMessageCommand> sendMessageCommands = new List<SendMessageCommand>();
                for (int i = 0; i < 3; i++)
                {
                    var sendMessageCommand = new SendMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = groupId1,
                        Content = "Test",
                        Type = 0,
                        SentBy = userId1,
                        Payload = Guid.NewGuid().ToString(),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "Number", Guid.NewGuid().ToString() } }
                    };
                    sendMessageCommands.Add(sendMessageCommand);
                    await mediator.SendAsync(sendMessageCommand);
                }
                var messages = await repository.ToListAsync<Core.Domain.Message>();
                var messageDtos = mapper.Map<IEnumerable<MessageDto>>(messages);
                foreach (var messageDto in messageDtos)
                {
                    messageDto.GroupId = groupId2;
                    messageDto.Content = "Test";
                    messageDto.Type = 1;
                    messageDto.SentBy = userId2;
                    messageDto.Payload = Guid.NewGuid().ToString();
                    messageDto.SentTime = Convert.ToDateTime("2020-1-1");
                    messageDto.IsSystem = true;
                    messageDto.IsRevoked = true;
                    messageDto.CustomProperties = new Dictionary<string, string> { { "Number", Guid.NewGuid().ToString() } };
                }
                var updateMessageDataCommand = new UpdateMessageDataCommand { Messages = messageDtos, UserId = Guid.NewGuid().ToString() };
                {
                    var response = await mediator.SendAsync<UpdateMessageDataCommand, SugarChatResponse>(updateMessageDataCommand);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(updateMessageDataCommand.UserId).Message);
                }
                updateMessageDataCommand.UserId = userId1;
                await mediator.SendAsync<UpdateMessageDataCommand, SugarChatResponse>(updateMessageDataCommand);
                var messagesUpdateAfter = await repository.ToListAsync<Core.Domain.Message>();
                foreach (var messageUpdateAfter in messagesUpdateAfter)
                {
                    var messageDto = messageDtos.FirstOrDefault(x => x.Id == messageUpdateAfter.Id);
                    var message = messages.FirstOrDefault(x => x.Id == messageUpdateAfter.Id);
                    messageUpdateAfter.GroupId.ShouldBe(messageDto.GroupId);
                    messageUpdateAfter.Content.ShouldBe(messageDto.Content);
                    messageUpdateAfter.Type.ShouldBe(messageDto.Type);
                    messageUpdateAfter.SentBy.ShouldBe(messageDto.SentBy);
                    messageUpdateAfter.SentTime.ShouldBe(messageDto.SentTime);
                    messageUpdateAfter.IsSystem.ShouldBe(messageDto.IsSystem);
                    messageUpdateAfter.Payload.ShouldBe(messageDto.Payload);
                    messageUpdateAfter.IsRevoked.ShouldBe(messageDto.IsRevoked);
                    messageUpdateAfter.CustomProperties.ShouldBe(messageDto.CustomProperties);
                    messageUpdateAfter.CreatedBy.ShouldBe(message.CreatedBy);
                    messageUpdateAfter.CreatedDate.ShouldBe(message.CreatedDate);
                }
            });
        }

        [Fact]
        public async Task ShouldGetListByIds()
        {
            await Run<IMediator, IRepository, IMessageDataProvider>(async (mediator, repository, messageDataProvider) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    var sendMessageCommand = new SendMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = Guid.NewGuid().ToString(),
                        Content = i.ToString(),
                        Type = 0,
                        SentBy = Guid.NewGuid().ToString(),
                        Payload = Guid.NewGuid().ToString(),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "Number", Guid.NewGuid().ToString() } }
                    };
                    await mediator.SendAsync(sendMessageCommand);
                }
                var messageIds = (await repository.ToListAsync<Core.Domain.Message>(x => new string[] { "1", "2", "5" }.Contains(x.Content))).Select(x => x.Id);
                var messages = await messageDataProvider.GetListByIdsAsync(messageIds);
                messages.Count().ShouldBe(2);
                foreach (var message in messages)
                {
                    new string[] { "1", "2" }.Contains(message.Content).ShouldBeTrue();
                }
            });
        }
    }
}