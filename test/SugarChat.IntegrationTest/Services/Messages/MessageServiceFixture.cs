using Mediator.Net;
using Shouldly;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Domain;
using System;
using SugarChat.Message.Basic;

namespace SugarChat.IntegrationTest.Services.Messages
{
    public class MessageServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUnreadMessageCount()
        {
            await Run<IMediator>(async (mediator) =>
            {
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(6);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        //ExcludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        //}
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        //IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        //}
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        //IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "AB" } } },
                        //    IsExactSearch = false
                        //}
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(6);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        //ExcludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        //},
                        //IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        //}
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(0);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupType = 10,
                        //ExcludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "3AB" } } }
                        //},
                        //IncludeGroupByGroupCustomProperties = new SearchGroupByGroupCustomPropertiesDto
                        //{
                        //    GroupCustomProperties = new Dictionary<string, List<string>> { { "A", new List<string> { "4AB" } } }
                        //}
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(1);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId2,
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
                {
                    var request = new GetUnreadMessageCountRequest()
                    {
                        UserId = userId,
                        GroupIds = new string[] { groups[3].Id },
                        GroupType = 10
                    };
                    var response = await mediator.RequestAsync<GetUnreadMessageCountRequest, SugarChatResponse<int>>(request);
                    response.Data.ShouldBe(3);
                }
            });
        }

        [Fact]
        public async Task ShouldSetMessageReadByUserBasedOnGroupId()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                for (int i = 0; i < 2; i++)
                {
                    var command = new SetMessageReadByUserBasedOnGroupIdCommand()
                    {
                        UserId = userId,
                        GroupId = groups[2].Id
                    };
                    var response = await mediator.SendAsync<SetMessageReadByUserBasedOnGroupIdCommand, SugarChatResponse>(command);
                    var groupUser = await repository.FirstOrDefaultAsync<GroupUser>(x => x.UserId == userId && x.GroupId == groups[2].Id);
                    var lastMessage = await repository.FirstOrDefaultAsync<Core.Domain.Message>(x => x.GroupId == groups[2].Id);
                    groupUser.UnreadCount.ShouldBe(0);
                }
            });
        }

        [Fact]
        public async Task ShouldGetMessagesByGroupIds()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var request = new GetMessagesByGroupIdsRequest()
                {
                    UserId = userId,
                    GroupIds = groups.Select(x => x.Id).ToArray()
                };
                var response = await mediator.RequestAsync<GetMessagesByGroupIdsRequest, SugarChatResponse<IEnumerable<MessageDto>>>(request);
                response.Data.Count().ShouldBe(9);
            });
        }

        [Fact]
        public async Task ShouldMigrateCustomProperty()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var messages = new List<Core.Domain.Message>();
                for (int i = 0; i < 35; i++)
                {
                    messages.Add(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "key1" + i, "value1" + i }, { "key2" + i, "value2" + i } }
                    });
                }
                for (int i = 0; i < 10; i++)
                {
                    messages.Add(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { }
                    });
                }
                for (int i = 0; i < 10; i++)
                {
                    messages.Add(new Core.Domain.Message
                    {
                        Id = Guid.NewGuid().ToString()
                    });
                }
                await repository.AddRangeAsync(messages);
                var response = await mediator.SendAsync<MigrateMessageCustomPropertyCommand, SugarChatResponse>(new MigrateMessageCustomPropertyCommand());
                (await repository.CountAsync<Core.Domain.Message>(x => x.CustomProperties != null && x.CustomProperties != new Dictionary<string, string>())).ShouldBe(0);
                var messageIds = messages.Select(x => x.Id).ToList();
                (await repository.CountAsync<MessageCustomProperty>(x => messageIds.Contains(x.MessageId))).ShouldBe(70);
            });
        }
    }
}
