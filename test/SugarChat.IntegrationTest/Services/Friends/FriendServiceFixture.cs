using Mediator.Net;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Friends
{
    public class FriendServiceFixture : TestBase
    {
        [Fact]
        public async Task ShouldAddFriend()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var response = await mediator.SendAsync<AddFriendCommand, SugarChatResponse>(new AddFriendCommand());
            });
        }

        [Fact]
        public async Task ShouldRemoveFriend()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var response = await mediator.SendAsync<RemoveFriendCommand, SugarChatResponse>(new RemoveFriendCommand());
            });
        }
    }
}
