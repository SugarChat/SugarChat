using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Requests;
using SugarChat.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest
{
    public class SugarChatClientFixture : TestBase
    {
        private readonly string _userId = Guid.NewGuid().ToString();
        private readonly string _groupId = Guid.NewGuid().ToString();

        [Fact]
        public async Task MockCreateUser()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                await client.CreateUserAsync(new AddUserCommand
                {
                    Id = _userId,
                    DisplayName = "TestUser"
                },
                default(CancellationToken));

                var result = await client.GetUserProfileAsync(new GetUserRequest { Id = _userId }, default(CancellationToken));
                result.Data.DisplayName.ShouldBe("TestUser");
            });
        }

        [Fact]
        public async Task MockUpdateMyProfile()
        {
            await Run<ISugarChatClient>(async (client) =>
            {
                await client.CreateUserAsync(new AddUserCommand
                {
                    Id = _userId,
                    DisplayName = "Test"
                },
                default(CancellationToken));

                await client.UpdateMyProfileAsync(new UpdateUserCommand
                {
                    Id = _userId,
                    DisplayName = "Update"
                },
                 default(CancellationToken));

                var result = await client.GetUserProfileAsync(new GetUserRequest { Id = _userId }, default(CancellationToken));
                result.Data.DisplayName.ShouldBe("Update");
            });
        }

        [Fact]
        public async Task MockCreateGroup()
        {
            await Run<ISugarChatClient, IRepository>(async (client, repository) =>
            {
                await client.CreateGroupAsync(new AddGroupCommand
                {
                    Id = _groupId,
                    Name = "TestGroup",
                    Description = "测试添加组"
                },
                default(CancellationToken));

                var group =await repository.FirstOrDefaultAsync<Group>(x => x.Id == _groupId,default(CancellationToken)).ConfigureAwait(false);
                group.Name.ShouldBe("TestGroup");
            });
        }

    }
}
