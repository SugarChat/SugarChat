﻿using Mediator.Net;
using SugarChat.Core.IRepositories;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
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
using SugarChat.Message.Requests;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.GroupUsers;

namespace SugarChat.IntegrationTest.Services
{
    public class MessageFixture : TestBase
    {
        string adminId = Guid.NewGuid().ToString();
        List<string> userIds = new List<string>();
        string groupId = Guid.NewGuid().ToString();

        [Fact]
        public async Task ShouldSendMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await InitGroup();
                object payload = new
                {
                    uuid = Guid.NewGuid(),
                    url = "testUrl",
                    size = 100,
                    second = 50
                };
                {
                    SendMessageCommand command = new SendMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = groupId,
                        Content = "Test",
                        Type = 5,
                        SentBy = adminId,
                        Payload = JsonConvert.SerializeObject(payload),
                        CreatedBy = adminId,
                        CustomProperties = new Dictionary<string, string> { { "Number", "1" } }
                    };
                    await mediator.SendAsync(command);
                    (await repository.AnyAsync<Core.Domain.Message>(x => x.GroupId == command.GroupId
                         && x.Content == command.Content
                         && x.Type == command.Type
                         && x.SentBy == command.SentBy
                         && x.Payload == command.Payload
                         && x.CreatedBy == command.CreatedBy)).ShouldBeTrue();
                    var message = await repository.SingleAsync<Core.Domain.Message>(x => x.Id == command.Id);
                    message.CustomProperties.GetValueOrDefault("Number").ShouldBe("1");
                    repository.Query<Group>().Single(x => x.Id == groupId).LastMessageId.ShouldBe(command.Id);
                }
                {
                    for (int i = 0; i < 5; i++)
                    {
                        SendMessageCommand command = new SendMessageCommand
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = groupId,
                            Content = "Test",
                            Type = 5,
                            SentBy = userIds.First(),
                            Payload = JsonConvert.SerializeObject(payload),
                            CreatedBy = userIds.First(),
                            IgnoreUnreadCountByGroupUserCustomProperties = new Dictionary<string, List<string>> { { "UserType", new List<string> { "Customer" } } }
                        };
                        await mediator.SendAsync(command);
                        repository.Query<Group>().Single(x => x.Id == groupId).LastMessageId.ShouldBe(command.Id);
                    }
                    repository.Query<GroupUser>().First(x => x.UserId == adminId).UnreadCount.ShouldBe(5);
                    repository.Query<GroupUser>().Where(x => userIds.Contains(x.UserId) && x.UnreadCount == 1).Count().ShouldBe(5);
                }
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
        public async Task ShouldNotGetRevokedMessages()
        {
            await Run<IMediator, IRepository, IMessageService>(async (mediator, repository, messageService) =>
            {
                string messageId1 = Guid.NewGuid().ToString();
                string messageId2 = Guid.NewGuid().ToString();
                string groupId = Guid.NewGuid().ToString();
                var message = new Core.Domain.Message
                {
                    Id = messageId1,
                    GroupId = groupId,
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

                var group = new Group() { Id = groupId };
                await repository.AddAsync(group);

                var messages = await messageService.GetMessagesOfGroupAsync(new GetMessagesOfGroupRequest()
                { GroupId = groupId });
                messages.Messages.Result.Count().ShouldBe(2);

                RevokeMessageCommand command = new RevokeMessageCommand
                {
                    MessageId = messageId2,
                    UserId = message.SentBy
                };

                await mediator.SendAsync(command);
                messages = await messageService.GetMessagesOfGroupAsync(new GetMessagesOfGroupRequest()
                { GroupId = groupId });
                messages.Messages.Result.Count().ShouldBe(1);
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
                var messages = await messageDataProvider.GetLastMessageByGroupIdsAsync(groupIds.Select(x => x.ToString()));
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
                var groupId1 = Guid.NewGuid().ToString();
                var groupId2 = Guid.NewGuid().ToString();

                await repository.AddAsync(new User
                {
                    Id = userId1
                });
                await repository.AddAsync(new User
                {
                    Id = userId2
                });
                foreach (var groupId in new string[] { groupId1, groupId2 })
                {
                    await repository.AddAsync(new Group
                    {
                        Id = groupId
                    });
                    foreach (var userId in new string[] { userId1, userId2 })
                    {
                        await repository.AddAsync(new GroupUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userId,
                            GroupId = groupId
                        });
                    }
                }

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
                    messageDto.CustomProperties = new Dictionary<string, string> { { "Number", "123456" }, { "A", "A" } };
                }
                var updateMessageDataCommand = new UpdateMessageDataCommand { Messages = messageDtos, UserId = Guid.NewGuid().ToString() };

                var response = await mediator.SendAsync<UpdateMessageDataCommand, SugarChatResponse>(updateMessageDataCommand);
                response.Message.ShouldBe(Prompt.UserNoExists.WithParams(updateMessageDataCommand.UserId).Message);

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
                    messageUpdateAfter.CreatedBy.ShouldBe(message.CreatedBy);
                    messageUpdateAfter.CreatedDate.ShouldBe(message.CreatedDate);
                    messageUpdateAfter.CustomProperties.Any(x => x.Key == "Number" && x.Value == "123456").ShouldBeTrue();
                }
            });
        }

        [Fact]
        public async Task ShouldGetListByIds()
        {
            await Run<IMediator, IRepository, IMessageDataProvider>(async (mediator, repository, messageDataProvider) =>
            {
                await InitGroup();
                for (int i = 0; i < 5; i++)
                {
                    var sendMessageCommand = new SendMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = groupId,
                        Content = i.ToString(),
                        Type = 0,
                        SentBy = adminId,
                        Payload = Guid.NewGuid().ToString(),
                        CreatedBy = adminId,
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

        private async Task InitGroup()
        {
            await Run<IMediator>(async mediator =>
            {
                await mediator.SendAsync(new AddUserCommand
                {
                    Id = adminId,
                });
                for (int i = 0; i < 5; i++)
                {
                    var userId = Guid.NewGuid().ToString();
                    await mediator.SendAsync(new AddUserCommand
                    {
                        Id = userId
                    });
                    userIds.Add(userId);
                }
                await mediator.SendAsync(new AddGroupCommand
                {
                    Id = groupId,
                    UserId = adminId,
                    CustomProperties = new Dictionary<string, string> { { "A", "1" }, { "B", "2" } }
                });
                await mediator.SendAsync(new SetGroupMemberCustomFieldCommand
                {
                    GroupId = groupId,
                    UserId = adminId,
                    CustomProperties = new Dictionary<string, string> { { "UserType", "Merchant" } }
                });
                await mediator.SendAsync(new AddGroupMemberCommand
                {
                    GroupId = groupId,
                    AdminId = adminId,
                    CreatedBy = adminId,
                    CustomProperties = new Dictionary<string, string> { { "UserType", "Customer" } },
                    GroupUserIds = userIds,
                    Role = Message.UserRole.Member
                });
            });
        }
    }
}