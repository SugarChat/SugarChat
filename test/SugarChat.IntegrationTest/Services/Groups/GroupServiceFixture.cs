using Mediator.Net;
using Shouldly;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Linq;
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

    }
}
