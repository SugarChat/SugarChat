using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos.Conversations;
using SugarChat.Shared.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Conversations
{
    public class ConversationServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUserConversations()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>
                (new GetConversationListRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 } });

                reponse.Data.Count().ShouldBe(3);
            });
        }

        [Fact]
        public async Task ShouldSetConversationMessagesRead()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
             {
                 var messageId = Guid.NewGuid().ToString();
                 await repository.AddAsync(new Core.Domain.Message
                 {
                     Id = messageId,
                     CreatedBy = Guid.NewGuid().ToString(),
                     CreatedDate = DateTimeOffset.Now,
                     LastModifyBy = Guid.NewGuid().ToString(),
                     CustomProperties = new Dictionary<string, string>(),
                     LastModifyDate = DateTimeOffset.Now,
                     GroupId = conversationId,
                     Content = "TestGroupMessageReaded",
                     ParsedContent = "TestGroupMessageReaded",
                     Type = MessageType.Text,
                     SubType = 0,
                     SentBy = Guid.NewGuid().ToString(), //用户3
                     SentTime = DateTimeOffset.Now,
                     IsSystem = true,
                     Payload = new { Text = "TestGroupMessageReaded" }
                 });

                 await mediator.SendAsync<SetMessageReadByUserBasedOnMessageIdCommand, SugarChatResponse>(new SetMessageReadByUserBasedOnMessageIdCommand
                 {
                     MessageId = messageId,
                     UserId = userId

                 }, default(CancellationToken));

                 var request = new GetConversationListRequest()
                 {
                     UserId = userId
                 };
                 var response = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(request);
                 response.Data.Where(x => x.ConversationID == conversationId)
                 .FirstOrDefault().UnreadCount.ShouldBe(0);
             });
        }

        [Fact]
        public async Task ShouldGetConversationProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetConversationProfileRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationProfileRequest, SugarChatResponse<ConversationDto>>(request);
                response.Data.GroupProfile.Name.ShouldBe("TestGroup3");
            });
        }

        [Fact]
        public async Task ShouldGetMessageList()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetMessageListRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    NextReqMessageId = "",
                    Count = 5
                };
                var response = await mediator.RequestAsync<GetMessageListRequest, SugarChatResponse<MessageListResult>>(request);
                response.Data.Messages.Count().ShouldBe(3);
                response.Data.Messages.First().Content.ShouldBe("[图片]");
            });
        }

        [Fact]
        public async Task ShouldDeleteConversation()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync<RemoveConversationCommand, SugarChatResponse>(new RemoveConversationCommand
                {
                    ConversationId = conversationId,
                    UserId = userId

                }, default(CancellationToken));

                var groupUser = await repository.SingleOrDefaultAsync<GroupUser>(x => x.GroupId == conversationId && x.UserId == userId);
                groupUser.ShouldBeNull();
            });
        }

        [Fact]
        public async Task ShouldGetConversationByKeyword()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new Shared.Paging.PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "Order", "1" } },
                        UserId = userId
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(requset);
                    response.Data.Count().ShouldBe(1);
                    response.Data.Any(x => x.ConversationID == conversationId).ShouldBeTrue();
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new Shared.Paging.PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "Order", "25" } },
                        UserId = userId,
                        IsExactSearch = true
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(requset);
                    response.Data.Count().ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new Shared.Paging.PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "Order", "11" }, { "Text", "test8" } },
                        UserId = userId,
                        IsExactSearch = true
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(requset);
                    response.Data.Count().ShouldBe(2);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new Shared.Paging.PageSettings { PageNum = 1, PageSize = 20 },
                        UserId = userId,
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<IEnumerable<ConversationDto>>>(requset);
                    response.Data.Count().ShouldBe(3);
                }
            });
        }
    }
}
