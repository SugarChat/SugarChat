using Mediator.Net;
using Shouldly;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.Groups;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Groups;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Groups
{
    public class GroupServiceFixture : TestFixtureBase
    {
        [Fact]
        public async Task ShouldGetUserGroups()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupsOfUserRequest, GetGroupsOfUserResponse>(new GetGroupsOfUserRequest { Id = userId });
                reponse.Groups.Count().ShouldBe(4);
            });
        }

        [Fact]
        public async Task ShouldGetGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, GetGroupProfileResponse>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Result.Name.ShouldBe("TestGroup3");
                reponse.Result.MemberCount.ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldUpdateGroupProfile()
        {
            await Run<IMediator>(async (mediator) =>
            {
                await mediator.SendAsync(new UpdateGroupProfileCommand
                {
                    Id = conversationId,
                    Name = "内部沟通群",
                    Description = "进行及时有效的沟通"

                }, default(CancellationToken));

                var reponse = await mediator.RequestAsync<GetGroupProfileRequest, GetGroupProfileResponse>(new GetGroupProfileRequest { UserId = userId, GroupId = conversationId });
                reponse.Result.Name.ShouldBe("内部沟通群");                
            });
        }

    }
}
