using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Paging;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class FriendDataProviderTests : ServiceFixture
    {
        private readonly IFriendDataProvider _friendDataProvider;

        public FriendDataProviderTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _friendDataProvider = Container.Resolve<IFriendDataProvider>();
        }

        [Fact]
        public async Task Should_Get_Friend_By_Its_Own_Id()
        {
            Friend friend = await _friendDataProvider.GetByOwnIdAsync(TomAndJerryFriend.Id);
            friend.ShouldBeEquivalentTo(TomAndJerryFriend);
        }

        [Fact]
        public async Task Should_Not_Get_Friend_By_Incorrect_Id()
        {
            Friend friend = await _friendDataProvider.GetByOwnIdAsync("0");
            friend.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Get_Friend_By_Both_Users_Ids()
        {
            Friend friend = await _friendDataProvider.GetByBothIdsAsync(Tom.Id, Jerry.Id);
            friend.ShouldBeEquivalentTo(TomAndJerryFriend);
        }

        [Fact]
        public async Task Should_Not_Get_Friend_By_Incorrect_Ids()
        {
            Friend friend = await _friendDataProvider.GetByBothIdsAsync(Tom.Id, Tyke.Id);
            friend.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Get_Paged_Friends_By_User_Id()
        {
            PagedResult<Friend> friend =
                await _friendDataProvider.GetAllFriendsByUserIdAsync(Tom.Id,
                    new PageSettings {PageNum = 1, PageSize = 1});
            friend.Total.ShouldBe(2);
            friend.Result.Count().ShouldBe(1);
            friend.Result.First().ShouldBeEquivalentTo(TomAndSpikeFriend);

            friend = await _friendDataProvider.GetAllFriendsByUserIdAsync(Tom.Id, new PageSettings {PageNum = 1});
            friend.Total.ShouldBe(2);
            friend.Result.Count().ShouldBe(2);
            friend.Result.First().ShouldBeEquivalentTo(TomAndSpikeFriend);
            friend.Result.Last().ShouldBeEquivalentTo(TomAndJerryFriend);
        }

        [Fact]
        public async Task Should_Add_None_Exist_Friend()
        {
            Friend tomAndTykeFriend = new Friend
            {
                Id = "0",
                UserId = Tom.Id,
                FriendId = Tyke.Id,
                BecomeFriendAt = BaseTime
            };
            await _friendDataProvider.AddAsync(tomAndTykeFriend);
            Friend friend = await _friendDataProvider.GetByOwnIdAsync(tomAndTykeFriend.Id);

            friend.ShouldBeEquivalentTo(tomAndTykeFriend);
        }

        [Fact]
        public async Task Should_Not_Add_Exist_Friend()
        {
            Friend tomAndTykeFriend = new Friend
            {
                Id = "1",
                UserId = Tom.Id,
                FriendId = Tyke.Id,
                BecomeFriendAt = BaseTime
            };
            await Assert.ThrowsAnyAsync<Exception>(async () => await _friendDataProvider.AddAsync(tomAndTykeFriend));
        }

        [Fact]
        public async Task Should_Update_Exist_Friend()
        {
            Friend tomAndJerryFriend = new Friend
            {
                Id = "1",
                UserId = Tom.Id,
                FriendId = Tyke.Id,
                BecomeFriendAt = BaseTime.AddMinutes(1)
            };
            await _friendDataProvider.UpdateAsync(tomAndJerryFriend);
            Friend friend = await _friendDataProvider.GetByOwnIdAsync(tomAndJerryFriend.Id);

            friend.ShouldBeEquivalentTo(tomAndJerryFriend);
        }

        [Fact]
        public async Task Should_Not_Update_None_Exist_Friend()
        {
            Friend tomAndTykeFriend = new Friend
            {
                Id = "0",
                UserId = Tom.Id,
                FriendId = Tyke.Id,
                BecomeFriendAt = BaseTime
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () => await _friendDataProvider.UpdateAsync(tomAndTykeFriend));
        }

        [Fact]
        public async Task Should_Remove_Exist_Friend()
        {
            Friend tomAndJerryFriend = new Friend
            {
                Id = "1"
            };
            await _friendDataProvider.RemoveAsync(tomAndJerryFriend);
            Friend friend = await _friendDataProvider.GetByOwnIdAsync(tomAndJerryFriend.Id);

            friend.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Not_Remove_None_Exist_Friend()
        {
            Friend tomAndTykeFriend = new Friend
            {
                Id = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () => await _friendDataProvider.RemoveAsync(tomAndTykeFriend));
        }
    }
}