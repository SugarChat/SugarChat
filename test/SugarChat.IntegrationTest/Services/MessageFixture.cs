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
using SugarChat.Message.Basic;
using AutoMapper;
using SugarChat.Message.Dtos;
using System.Linq;
using SugarChat.Core.Domain;

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
        public async Task ShouldUpdateMessage()
        {
            await Run<IMediator, IRepository, IMapper>(async (mediator, repository, mapper) =>
            {
                var userId = Guid.NewGuid().ToString();
                await repository.AddAsync(new User
                {
                    Id = userId
                });
                List<SendMessageCommand> sendMessageCommands = new List<SendMessageCommand>();
                for (int i = 0; i < 3; i++)
                {
                    var sendMessageCommand = new SendMessageCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupId = Guid.NewGuid().ToString(),
                        Content = "Test",
                        Type = 0,
                        SentBy = Guid.NewGuid().ToString(),
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
                    messageDto.GroupId = Guid.NewGuid().ToString();
                    messageDto.Content = "Test";
                    messageDto.Type = 1;
                    messageDto.SentBy = Guid.NewGuid().ToString();
                    messageDto.Payload = Guid.NewGuid().ToString();
                    messageDto.CustomProperties = new Dictionary<string, string> { { "Number", Guid.NewGuid().ToString() } };
                }
                var updateMessageCommand = new UpdateMessageCommand { Messages = messageDtos, UserId = Guid.NewGuid().ToString() };
                {
                    var response = await mediator.SendAsync<UpdateMessageCommand, SugarChatResponse>(updateMessageCommand);
                    response.Message.ShouldBe(Prompt.UserNoExists.WithParams(updateMessageCommand.UserId).Message);
                }
                updateMessageCommand.UserId = userId;
                await mediator.SendAsync<UpdateMessageCommand, SugarChatResponse>(updateMessageCommand);
                var messagesUpdateAfter = await repository.ToListAsync<Core.Domain.Message>();
                foreach (var _messages in messagesUpdateAfter)
                {
                    var messageDto = messageDtos.FirstOrDefault(x => x.Id == _messages.Id);
                    var message = messages.FirstOrDefault(x => x.Id == _messages.Id);
                    _messages.GroupId.ShouldBe(messageDto.GroupId);
                    _messages.Content.ShouldBe(messageDto.Content);
                    _messages.Type.ShouldBe(messageDto.Type);
                    _messages.SentBy.ShouldBe(messageDto.SentBy);
                    _messages.Payload.ShouldBe(messageDto.Payload);
                    _messages.CustomProperties.ShouldBe(messageDto.CustomProperties);
                    _messages.CreatedBy.ShouldBe(message.CreatedBy);
                    _messages.CreatedDate.ShouldBe(message.CreatedDate);
                    _messages.SentTime.ShouldBe(message.SentTime);
                    _messages.IsSystem.ShouldBe(message.IsSystem);
                    _messages.IsRevoked.ShouldBe(message.IsRevoked);
                }
            });
        }
    }
}