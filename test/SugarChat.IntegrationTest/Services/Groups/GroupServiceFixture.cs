using Mediator.Net;
using Shouldly;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Paging;
using Xunit;
using SugarChat.Message.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;

namespace SugarChat.IntegrationTest.Services.Groups
{
    public class GroupServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUserGroups()
        {
            await Run<IMediator>(async (mediator) =>
            {
                {
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1 }, Type = 10 });
                    reponse.Data.Result.Count().ShouldBe(5);
                }
                {
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize = 2 }, Type = 10 });
                    reponse.Data.Result.Count().ShouldBe(2);
                }
                {
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = null, Type = 10 });
                    reponse.Data.Result.Count().ShouldBe(5);
                }
            });
        }

        [Fact]
        public async Task ShouldGetGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Name.ShouldBe("TestGroup3");
                reponse.Data.MemberCount.ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldUpdateGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                await mediator.SendAsync<UpdateGroupProfileCommand, SugarChatResponse>(new UpdateGroupProfileCommand
                {
                    Id = conversationId,
                    Name = "内部沟通群",
                    Description = "进行及时有效的沟通",
                    Type = 10

                }, default(CancellationToken));

                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Name.ShouldBe("内部沟通群");
                reponse.Data.Type.ShouldBe(10);
            });
        }

        [Fact]
        public async Task ShouldRemoveGroup()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var response = await mediator.SendAsync<RemoveGroupCommand, SugarChatResponse>(new RemoveGroupCommand());
            });
        }

        [Fact]
        public async Task ShouldMigrateCustomProperty()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var groups = new List<Group>();
                for (int i = 0; i < 35; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { { "key1" + i, "value1" + i }, { "key2" + i, "value2" + i } }
                    });
                }
                for (int i = 0; i < 10; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomProperties = new Dictionary<string, string> { }
                    });
                }
                for (int i = 0; i < 10; i++)
                {
                    groups.Add(new Group
                    {
                        Id = Guid.NewGuid().ToString()
                    });
                }
                await repository.AddRangeAsync(groups);
                var response = await mediator.SendAsync<MigrateGroupCustomPropertyCommand, SugarChatResponse>(new MigrateGroupCustomPropertyCommand());
                (await repository.CountAsync<Group>(x => x.CustomProperties != new Dictionary<string, string> { } && x.CustomProperties != null)).ShouldBe(0);
                var groupIds = groups.Select(x => x.Id).ToList();
                (await repository.CountAsync<GroupCustomProperty>(x => groupIds.Contains(x.GroupId))).ShouldBe(70);
            });
        }
    }
}
