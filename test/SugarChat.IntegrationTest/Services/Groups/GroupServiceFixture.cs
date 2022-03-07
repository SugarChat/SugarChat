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
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1 } });
                    reponse.Data.Result.Count().ShouldBe(5);
                }
                {
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = new PageSettings { PageNum = 1, PageSize =2} });
                    reponse.Data.Result.Count().ShouldBe(2);
                }
                {
                    var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, SugarChatResponse<PagedResult<GroupDto>>>(new GetGroupsOfUserRequest { UserId = userId, PageSettings = null });
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
                    Description = "进行及时有效的沟通"

                }, default(CancellationToken));

                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, SugarChatResponse<GroupDto>>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Data.Name.ShouldBe("内部沟通群");
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
    }
}
