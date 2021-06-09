using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Message.Commands.Friends;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
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
            FriendAddedEvent friendAddedEvent =
                await _friendService.AddFriendAsync(new AddFriendCommand {UserId = Tom.Id, FriendId = Tyke.Id});
            Friend friend = await Repository.SingleOrDefaultAsync<Friend>(o => o.Id == friendAddedEvent.Id);
            friend.ShouldNotBeNull();
            friend.UserId.ShouldBe(Tom.Id);
            friend.FriendId.ShouldBe(Tyke.Id);
            friendAddedEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_They_Already_Are()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _friendService.AddFriendAsync(new AddFriendCommand {UserId = Tom.Id, FriendId = Jerry.Id}));
        }


        [Fact]
        public async Task Should_Not_Add_Friend_When_They_Are_Not_Legal_User()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _friendService.AddFriendAsync(new AddFriendCommand {UserId = "0", FriendId = Jerry.Id}));
        }

        [Fact]
        public async Task Should_Not_Add_Self_As_Friend()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _friendService.AddFriendAsync(new AddFriendCommand {UserId = Jerry.Id, FriendId = Jerry.Id}));
        }

        [Fact]
        public async Task Should_Remove_Friend_When_They_Are()
        {
            FriendRemovedEvent removeFriendEvent =
                await _friendService.RemoveFriendAsync(new RemoveFriendCommand {UserId = Tom.Id, FriendId = Jerry.Id});
            Friend friend = await Repository.SingleOrDefaultAsync<Friend>(o =>
                o.UserId == removeFriendEvent.UserId && o.FriendId == removeFriendEvent.FriendId);
            friend.ShouldBeNull();
            removeFriendEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Remove_Friend_When_They_Are_Not()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _friendService.RemoveFriendAsync(new RemoveFriendCommand {UserId = Tom.Id, FriendId = Tyke.Id}));
        }
    }
}