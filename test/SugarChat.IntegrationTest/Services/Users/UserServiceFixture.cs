using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Users
{
    public class UserServiceFixture : TestFixtureBase
    {       
        [Fact]
        public async Task ShouldGetUserProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var reponse = await mediator.RequestAsync<GetUserRequest, GetUserResponse>(new GetUserRequest { Id = userId });
                reponse.User.DisplayName.ShouldBe("TestUser10");
            });
        }      

        [Fact]
        public async Task ShouldUpdateMyProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new UpdateUserCommand
                {
                    Id = userId,
                    DisplayName = "UpdateUserProfileTest",                   
                    CustomProperties = new Dictionary<string, string>
                    {
                        { "selfSignature", "我的个性签名" }
                    }

                }, default(CancellationToken));

                var user = await repository.SingleOrDefaultAsync<User>(x => x.Id == userId);
                user.CustomProperties["selfSignature"].ShouldBe("我的个性签名");
                user.DisplayName.ShouldBe("UpdateUserProfileTest");
            });
        }

       


    }
}
