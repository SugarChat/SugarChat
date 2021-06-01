using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class FriendServiceTests : ServiceFixture
    {
        private readonly IFriendService _friendService;

        public FriendServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _friendService = Container.Resolve<IFriendService>();
        }

        [Fact]
        public async Task Should_Add_Friend_When_They_Are_Not()
        {
            AddFriendEvent addFriendEvent =
                await _friendService.AddFriendAsync(new AddFriendCommand {UserId = Tom.Id, FriendId = Tyke.Id});
            Friend friend = await Repository.SingleOrDefaultAsync<Friend>(o => o.Id == addFriendEvent.Id);
            friend.ShouldNotBeNull();
            friend.UserId.ShouldBe(Tom.Id);
            friend.FriendId.ShouldBe(Tyke.Id);
            addFriendEvent.Status.ShouldBe(EventStatus.Success);
        }
    }
}