using Mediator.Net;
using Shouldly;
using SugarChat.Core.Basic;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Users
{
    public class UserServiceFixture : TestFixtureBase
    {
        protected User Tom;

        private async Task AddUser(IRepository repository)
        {
            Tom = new()
            {
                Id = "1",
                DisplayName = "Tom"
            };
            await repository.AddAsync(Tom);
        }
        [Fact]
        public async Task ShouldGetUserProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var reponse = await mediator.RequestAsync<GetUserRequest, SugarChatResponse<UserDto>>(new GetUserRequest { Id = userId });
                reponse.Data.DisplayName.ShouldBe("TestUser10");
            });
        }

        [Fact]
        public async Task ShouldUpdateMyProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync<UpdateUserCommand, SugarChatResponse>(new UpdateUserCommand
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

        [Fact]
        public async Task ShouldAddUser()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                AddUserCommand addUserCommand = new()
                {
                    Id = "0",
                    DisplayName = "Micky"
                };
                var response = await mediator.SendAsync<AddUserCommand, SugarChatResponse>(addUserCommand);
                response.Code.ShouldBe(20000);
                User user = await repository.SingleOrDefaultAsync<User>(o => o.Id == addUserCommand.Id);
                user.ShouldNotBeNull();
                user.Id.ShouldBe(addUserCommand.Id);
                user.DisplayName.ShouldBe(addUserCommand.DisplayName);
            });
        }

        [Fact]
        public async Task ShouldRemoveUser()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await AddUser(repository);
                RemoveUserCommand removeUserCommand = new()
                {
                    Id = Tom.Id
                };
                var response = await mediator.SendAsync<RemoveUserCommand, SugarChatResponse>(removeUserCommand);
                (await repository.SingleOrDefaultAsync<User>(o => o.Id == removeUserCommand.Id)).ShouldBeNull();
            });
        }

        [Fact]
        public async Task ShouldGetCurrentUser()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                GetCurrentUserRequest getCurrentUserRequest = new();
                var reponse = await mediator.RequestAsync<GetCurrentUserRequest, SugarChatResponse<UserDto>>(getCurrentUserRequest);
                reponse.Data.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ShouldGetFriendsOfUser()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var reponse = await mediator.RequestAsync<GetFriendsOfUserRequest, SugarChatResponse<PagedResult<UserDto>>>(new GetFriendsOfUserRequest());
            });
        }
    }
}
