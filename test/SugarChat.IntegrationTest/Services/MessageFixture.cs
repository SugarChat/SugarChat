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
                    CustomProperties = new Dictionary<string, string> { { "Number", "1" } },
                    FileUrl = Guid.NewGuid().ToString()
                };
                await mediator.SendAsync(command);
                var message = await repository.SingleAsync<Core.Domain.Message>(x => x.GroupId == command.GroupId
                     && x.Content == command.Content
                     && x.Type == command.Type
                     && x.SentBy == command.SentBy
                     && x.Payload == command.Payload
                     && x.CreatedBy == command.CreatedBy
                     && x.CustomProperties == command.CustomProperties
                     && x.FileUrl == command.FileUrl);
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
                List<User> users = new List<User>();
                List<GroupUser> groupUsers = new List<GroupUser>();
                await repository.AddAsync(new Core.Domain.Message
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = groupId,
                    SentTime = lastMessageSentTime
                });

                {
                    string userId = Guid.NewGuid().ToString();
                    groupUsers.Add(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = groupId,
                        UserId = userId,
                        Role = UserRole.Owner
                    });
                    users.Add(new User
                    {
                        Id = userId
                    });
                }

                {
                    for (int i = 0; i < 10; i++)
                    {
                        string userId = Guid.NewGuid().ToString();
                        groupUsers.Add(new GroupUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            GroupId = groupId,
                            UserId = userId
                        });
                        users.Add(new User
                        {
                            Id = userId
                        });
                    }
                }

                await repository.AddRangeAsync(users);
                await repository.AddRangeAsync(groupUsers);
                IEnumerable<string> userIds = groupUsers.Select(x => x.UserId);

                SetMessageReadByUserIdsBasedOnGroupIdCommand command = new SetMessageReadByUserIdsBasedOnGroupIdCommand()
                {
                    UserId = Guid.NewGuid().ToString(),
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
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(command.UserId).Message);
                }
                {
                    command.UserId = groupUsers[1].UserId.ToString();
                    var response = await mediator.SendAsync<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotAdmin.WithParams(command.UserId, command.GroupId).Message);
                }
                {
                    command.UserId = groupUsers[0].UserId.ToString();
                    var response = await mediator.SendAsync<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>(command);
                    response.Message.ShouldBe(Prompt.NotAllUsersExists.Message);
                }
                {
                    command.UserIds = userIds;
                    await mediator.SendAsync(command);
                    (await repository.CountAsync<GroupUser>(x => x.GroupId == groupId && userIds.Contains(x.UserId) && x.LastReadTime == lastMessageSentTime)).ShouldBe(11);
                }
            });
        }

        [Fact]
        public async Task ShouldTranslateMessage()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                {
                    var message = new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = "莫忘记，人类情感上最大的需要是感恩。",
                    };
                    await repository.AddAsync(message);
                    var command = new TranslateMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedBy = Guid.NewGuid().ToString(),
                        LanguageCode = "aaa",
                        MessageId = Guid.NewGuid().ToString()
                    };
                    {
                        var response = await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                        response.Message.ShouldBe(Prompt.LanguageCodeIsWrong.Message);
                    }
                    {
                        command.LanguageCode = "en-US";
                        var response = await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                        response.Message.ShouldBe(Prompt.MessageNoExists.WithParams(command.MessageId).Message);
                    }
                    {
                        command.MessageId = message.Id;
                        await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                        (await repository.CountAsync<MessageTranslate>(x => x.MessageId == message.Id && x.LanguageCode == command.LanguageCode)).ShouldBe(1);
                    }
                    {
                        await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                        (await repository.CountAsync<MessageTranslate>(x => x.MessageId == message.Id && x.LanguageCode == command.LanguageCode)).ShouldBe(1);
                    }
                    {
                        command.LanguageCode = "es-ES";
                        command.Id = Guid.NewGuid().ToString();
                        await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                        (await repository.CountAsync<MessageTranslate>(x => x.MessageId == message.Id && x.LanguageCode == command.LanguageCode)).ShouldBe(1);
                    }
                }
                {
                    var message = new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = "Cero Miedo",
                    };
                    await repository.AddAsync(message);
                    var command = new TranslateMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedBy = Guid.NewGuid().ToString(),
                        LanguageCode = "zh-TW",
                        MessageId = message.Id
                    };
                    await mediator.SendAsync<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>(command);
                    (await repository.CountAsync<MessageTranslate>(x => x.MessageId == message.Id && x.LanguageCode == command.LanguageCode && x.Content == "零恐懼")).ShouldBe(1);
                }
            });
        }
    }
}