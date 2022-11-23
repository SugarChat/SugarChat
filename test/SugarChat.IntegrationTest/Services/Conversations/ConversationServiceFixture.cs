using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Message.Dtos;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Core.Services.Conversations;

namespace SugarChat.IntegrationTest.Services.Conversations
{
    public class ConversationServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUserConversations()
        {
            await Run<IMediator>(async (mediator) =>
            {
                {
                    var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetConversationListRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(5);
                }
                {
                    var reponse = await mediator.RequestAsync<GetUnreadConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetUnreadConversationListRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(3);
                }
                {
                    var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetConversationListRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupIds = new string[] { conversationId, Guid.NewGuid().ToString() }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(1);
                }
                {
                    var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetConversationListRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupIds = new string[] { Guid.NewGuid().ToString() }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(0);
                }
                {
                    var reponse = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetConversationListRequest { UserId = userId9, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(2);
                }
                {
                    var reponse = await mediator.RequestAsync<GetUnreadConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>
                    (new GetUnreadConversationListRequest { UserId = userId9, PageSettings = new PageSettings { PageNum = 1, PageSize = 10 }, GroupType = 10 });
                    reponse.Data.Result.Count().ShouldBe(1);
                }
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
                     LastModifyDate = DateTimeOffset.Now,
                     GroupId = conversationId,
                     Content = "TestGroupMessageRead",
                     Type = 0,
                     SentBy = Guid.NewGuid().ToString(), //用户3
                     SentTime = DateTimeOffset.Now,
                     IsSystem = true,
                     Payload = "{\"Text\":\"TestGroupMessageRead\"}"
                 });

                 await mediator.SendAsync<SetMessageReadByUserBasedOnMessageIdCommand, SugarChatResponse>(new SetMessageReadByUserBasedOnMessageIdCommand
                 {
                     MessageId = messageId,
                     UserId = userId

                 }, default(CancellationToken));

                 var request = new GetConversationListRequest()
                 {
                     UserId = userId,
                     PageSettings = new PageSettings { PageNum = 1, PageSize = 10 },
                     GroupType = 10
                 };
                 var response = await mediator.RequestAsync<GetConversationListRequest, SugarChatResponse<PagedResult<ConversationDto>>>(request);
                 response.Data.Result.Where(x => x.ConversationID == conversationId).FirstOrDefault().UnreadCount.ShouldBe(0);
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
                {
                    var request = new GetMessageListRequest()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        NextReqMessageId = "",
                        Count = 5
                    };
                    var response = await mediator.RequestAsync<GetMessageListRequest, SugarChatResponse<GetMessageListResponse>>(request);
                    response.Data.Messages.Count().ShouldBe(3);
                    response.Data.Messages.First().Content.ShouldBe("[图片]");
                }
                {
                    var request = new GetMessageListRequest()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Index = 1,
                        Count = 2
                    };
                    var response = await mediator.RequestAsync<GetMessageListRequest, SugarChatResponse<GetMessageListResponse>>(request);
                    response.Data.Messages.Count().ShouldBe(2);
                }
                {
                    var request = new GetMessageListRequest()
                    {
                        ConversationId = conversationId,
                        UserId = userId,
                        Index = 2,
                        Count = 2
                    };
                    var response = await mediator.RequestAsync<GetMessageListRequest, SugarChatResponse<GetMessageListResponse>>(request);
                    response.Data.Messages.Count().ShouldBe(1);
                }
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
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "1" } },
                        UserId = userId
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(0);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "1" } },
                        UserId = userId,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(1);
                    response.Data.Result.Any(x => x.ConversationID == conversationId).ShouldBeTrue();
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "25" } },
                        UserId = userId,
                        IsExactSearch = true,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(1);
                    response.Data.Total.ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "11" }, { "text", "test1" }, { "Content", "是" } },
                        UserId = userId,
                        IsExactSearch = false,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(2);
                    response.Data.Total.ShouldBe(2);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "11" }, { "text", "test8" }, { "Content", "是" } },
                        UserId = userId,
                        IsExactSearch = true,
                        GroupIds = new string[] { conversationId, groupId2, groupId4 },
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(0);
                    response.Data.Total.ShouldBe(0);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        UserId = userId,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(5);
                    response.Data.Total.ShouldBe(5);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "Content", "Congratulations! Your friend 六角恐龙～+. had completed an order, you are awarded 100 points from QC Test Store!" } },
                        UserId = userId,
                        IsExactSearch = false,
                        GroupIds = new string[] { conversationId, groupId2, groupId4, groupId5 },
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(1);
                    response.Data.Total.ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        UserId = userId,
                        IsExactSearch = true,
                        GroupIds = new string[] { conversationId, groupId2, groupId4 },
                        GroupType = 10,
                        IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        {
                            GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        }
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(1);
                    response.Data.Total.ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        UserId = userId,
                        IsExactSearch = true,
                        GroupIds = new string[] { conversationId, groupId2, groupId4 },
                        GroupType = 10,
                        IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        {
                            GroupCustomProperties = new Dictionary<string, List<string>> { { "B", new List<string> { "0BC" } } }
                        }
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(1);
                    response.Data.Total.ShouldBe(1);
                }
                {
                    GetConversationByKeywordRequest requset = new GetConversationByKeywordRequest
                    {
                        PageSettings = new PageSettings { PageNum = 1, PageSize = 20 },
                        SearchParms = new Dictionary<string, string> { { "order", "11" }, { "text", "test1" }, { "Content", "是" } },
                        UserId = userId,
                        IsExactSearch = false,
                        GroupIds = new string[] { conversationId, groupId2, groupId4 },
                        GroupType = 10,
                        IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        {
                            GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "2AB", "3AB" } } }
                        }
                    };
                    var response = await mediator.RequestAsync<GetConversationByKeywordRequest, SugarChatResponse<PagedResult<ConversationDto>>>(requset);
                    response.Data.Result.Count().ShouldBe(2);
                    response.Data.Total.ShouldBe(2);
                }
            });
        }

        [Fact]
        public async Task ShouldGetGroupIdsByMessageKeyword()
        {
            await Run<IMediator, IRepository, IGroupDataProvider>(async (mediator, repository, groupDataProvider) =>
            {
                {
                    var result = await groupDataProvider.GetGroupIdsByMessageKeywordAsync(new string[] { conversationId, groupId4, groupId5 }, new Dictionary<string, string> { { "order", "11" } }, true, 10, default);
                    result.Count().ShouldBe(1);
                    result.FirstOrDefault().ShouldBe(conversationId);
                }
                {
                    var result = await groupDataProvider.GetGroupIdsByMessageKeywordAsync(new string[] { conversationId, groupId4, groupId5 }, new Dictionary<string, string> { { "order", "2" } }, false, 10, default);
                    result.Count().ShouldBe(2);
                    result.ShouldContain(conversationId);
                    result.ShouldContain(groupId4);
                }
                {
                    var result = await groupDataProvider.GetGroupIdsByMessageKeywordAsync(new string[] { conversationId, groupId4 }, new Dictionary<string, string> { { "order", "11" }, { "text", "test1" }, { "Content", "是" } }, false, 10, default);
                    result.Count().ShouldBe(2);
                }
            });
        }

        [Fact]
        public async Task ShouldGetMessageUnreadCountGroupByGroupIds()
        {
            await Run<IMediator, IRepository, IMessageDataProvider>(async (mediator, repository, messageDataProvider) =>
            {
                {
                    var result = await messageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync(userId, new string[] { conversationId, groupId4, groupId5 }, new PageSettings(), 10);
                    result.Count().ShouldBe(3);
                    result.FirstOrDefault(x => x.GroupId == conversationId).UnreadCount.ShouldBe(2);
                    result.FirstOrDefault(x => x.GroupId == groupId4).UnreadCount.ShouldBe(3);
                    result.FirstOrDefault(x => x.GroupId == groupId5).UnreadCount.ShouldBe(1);

                    await repository.AddAsync(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTimeOffset.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTimeOffset.Now,
                        GroupId = conversationId,
                        Content = "TestGroupMessageRead",
                        Type = 0,
                        SentBy = Guid.NewGuid().ToString(),
                        SentTime = DateTimeOffset.Now.AddMinutes(10),
                        IsSystem = true,
                        Payload = "{\"Text\":\"TestGroupMessageRead\"}"
                    });
                }
                {
                    var result = await messageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync(userId, new string[] { conversationId, groupId4, groupId5 }, new PageSettings(), 10);
                    result.Count().ShouldBe(3);
                    result.FirstOrDefault(x => x.GroupId == conversationId).UnreadCount.ShouldBe(2);
                    result.FirstOrDefault(x => x.GroupId == groupId4).UnreadCount.ShouldBe(3);
                    result.FirstOrDefault(x => x.GroupId == groupId5).UnreadCount.ShouldBe(1);
                }
            });
        }

        [Fact]
        public async Task ShouldGetLastMessageBygGroupId()
        {
            await Run<IMediator, IRepository, IMessageDataProvider>(async (mediator, repository, messageDataProvider) =>
            {
                {
                    var lastMessage = await messageDataProvider.GetLastMessageBygGroupIdAsync(conversationId);
                    lastMessage.Content.ShouldBe("[图片]");
                }
                {
                    var lastMessage = await messageDataProvider.GetLastMessageBygGroupIdAsync(groupId4);
                    lastMessage.Content.ShouldBe("谁说不是呢");
                }
                {
                    var lastMessage = await messageDataProvider.GetLastMessageBygGroupIdAsync(groupId5);
                    lastMessage.Content.ShouldBe("888");
                }
            });
        }
    }
}
