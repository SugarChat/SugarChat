using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Users;
using SugarChat.Core.Settings;
using SugarChat.Data.MongoDb;
using Xunit;

namespace SugarChat.IntegrationTest.UserServiceTest
{
    public class UerServiceTest : TestBase
    {
        private readonly IRepository _repository;
        private readonly IUserService _userService;
        private readonly User _user;
        private readonly User _friend;
        private readonly DateTimeOffset _createDateTime = DateTimeOffset.UtcNow;
        private readonly string _userId = Guid.NewGuid().ToString();
        private readonly string _friendId = Guid.NewGuid().ToString();
        private readonly string _creator = Guid.NewGuid().ToString();
        private readonly string _modifier = Guid.NewGuid().ToString();


        public UerServiceTest()
        {
            MongoDbSettings settings = new MongoDbSettings();
            _configuration.GetSection("MongoDb")
                .Bind(settings);
            _repository = new MongoDbRepository(settings);

            _userService = new UserService(_repository);
            _user = new User
            {
                Id = _userId,
                AvatarUrl = "https://test.com/tom",
                CreatedBy = _creator,
                CreatedDate = _createDateTime,
                CustomProperties = new Dictionary<string, string>(),
                DisplayName = "Tom",
                LastModifyBy = _modifier,
                LastModifyDate = _createDateTime
            };

            _friend = new User
            {
                Id = _friendId,
                AvatarUrl = "https://test.com/jerry",
                CreatedBy = _creator,
                CreatedDate = _createDateTime,
                CustomProperties = new Dictionary<string, string>(),
                DisplayName = "Jerry",
                LastModifyBy = _modifier,
                LastModifyDate = _createDateTime
            };
        }

        [Fact]
        public async Task Should_Add_New_User()
        {
            await CleanUser(_user);
            try
            {
                await _userService.AddAsync(_user, CancellationToken.None);
                (await _repository.SingleAsync<User>(o => o.Id == _userId)).ShouldNotBeNull();
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Add_Null_User()
        {
            await CleanUser(_user);
            try
            {
                await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    await _userService.AddAsync(null, CancellationToken.None));
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Reject_To_Add_Existed_User()
        {
            await PrepareUser(_user);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddAsync(_user, CancellationToken.None));
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Delete_Exist_User()
        {
            await PrepareUser(_user);
            try
            {
                await _userService.DeleteAsync(_user.Id, CancellationToken.None);
                (await _repository.SingleOrDefaultAsync<User>(o => o.Id == _userId)).ShouldBeNull();
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Delete_None_Exist_User()
        {
            await CleanUser(_user);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.DeleteAsync(_user.Id, CancellationToken.None));
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Get_Exist_User()
        {
            await PrepareUser(_user);
            try
            {
                User user = await _userService.GetAsync(_user.Id);
                user.ShouldNotBeNull();
                user.Id.ShouldBe(_userId);
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_User()
        {
            await CleanUser(_user);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetAsync(_user.Id));
            }
            finally
            {
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Add_Friend()
        {
            await CleanFriend();
            await PrepareUser(_user);
            await PrepareUser(_friend);
            try
            {
                await _userService.AddFriendAsync(_user.Id, _friend.Id, CancellationToken.None);
                Friend friend =
                    await _repository.SingleAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id);
                friend.ShouldNotBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
                await CleanUser(_friend);
            }
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_User_Not_Exist()
        {
            await CleanFriend();
            await CleanUser(_user);
            await PrepareUser(_friend);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
                Friend friend =
                    await _repository.SingleOrDefaultAsync<Friend>(
                        o => o.UserId == _user.Id && o.FriendId == _friend.Id);
                friend.ShouldBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_friend);
            }
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Friend_Not_Exist()
        {
            await CleanFriend();
            await PrepareUser(_user);
            await CleanUser(_friend);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
                Friend friend =
                    await _repository.SingleOrDefaultAsync<Friend>(
                        o => o.UserId == _user.Id && o.FriendId == _friend.Id);
                friend.ShouldBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Both_Not_Exist()
        {
            await CleanFriend();
            await CleanUser(_user);
            await CleanUser(_friend);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
                Friend friend =
                    await _repository.SingleOrDefaultAsync<Friend>(
                        o => o.UserId == _user.Id && o.FriendId == _friend.Id);
                friend.ShouldBeNull();
            }
            finally
            {
                await CleanFriend();
            }
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Add_Self()
        {
            await CleanFriend();
            await PrepareUser(_user);
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddFriendAsync(_user.Id, _user.Id, CancellationToken.None));
                Friend friend =
                    await _repository.SingleOrDefaultAsync<Friend>(
                        o => o.UserId == _user.Id && o.FriendId == _friend.Id);
                friend.ShouldBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Relation_Already_Exists()
        {
            await PrepareUser(_user);
            await PrepareUser(_friend);
            await PrepareFriend();
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.AddFriendAsync(_user.Id, _friend.Id, CancellationToken.None));
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
                await CleanUser(_friend);
            }
        }

        [Fact]
        public async Task Should_Remove_Friend()
        {
            await PrepareUser(_user);
            await PrepareUser(_friend);
            await PrepareFriend();

            try
            {
                await _userService.RemoveFriendAsync(_user.Id, _friendId);
                (await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id))
                    .ShouldBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
                await CleanUser(_friend);
            }
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_User_Not_Exist()
        {
            await CleanUser(_user);
            await PrepareUser(_friend);
            await PrepareFriend();

            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.RemoveFriendAsync(_user.Id, _friendId));
                (await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id))
                    .ShouldNotBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_friend);
            }
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Friend_Not_Exist()
        {
            await PrepareUser(_user);
            await CleanUser(_friend);
            await PrepareFriend();

            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.RemoveFriendAsync(_user.Id, _friendId));
                (await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id))
                    .ShouldNotBeNull();
            }
            finally
            {
                await CleanFriend();
                await CleanUser(_user);
            }
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Both_Not_Exist()
        {
            await CleanUser(_user);
            await CleanUser(_friend);
            await PrepareFriend();

            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.RemoveFriendAsync(_user.Id, _friendId));
                (await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id))
                    .ShouldNotBeNull();
            }
            finally
            {
                await CleanFriend();
            }
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Relation_Dose_Not_Exists()
        {
            await CleanFriend();
            
            try
            {
                await Assert.ThrowsAsync<BusinessException>(async () =>
                    await _userService.RemoveFriendAsync(_user.Id, _friendId));
                (await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id))
                    .ShouldBeNull();
            }
            finally
            {
            }
        }

        private async Task CleanUser(User user)
        {
            User userInRepo = await _repository.SingleOrDefaultAsync<User>(o => o.Id == user.Id);
            if (userInRepo is not null)
            {
                await _repository.RemoveAsync(_user, CancellationToken.None);
            }
        }

        private async Task PrepareUser(User user)
        {
            User userInRepo = await _repository.SingleOrDefaultAsync<User>(o => o.Id == user.Id);
            if (userInRepo is null)
            {
                await _repository.AddAsync(user, CancellationToken.None);
            }
        }

        private async Task CleanFriend()
        {
            Friend friend =
                await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id);
            if (friend is not null)
            {
                await _repository.RemoveAsync(friend, CancellationToken.None);
            }
        }

        private async Task PrepareFriend()
        {
            Friend friend =
                await _repository.SingleOrDefaultAsync<Friend>(o => o.UserId == _user.Id && o.FriendId == _friend.Id);
            if (friend is null)
            {
                await _repository.AddAsync(new Friend
                {
                    Id = Guid.NewGuid().ToString(),
                    BecomeFriendAt = DateTimeOffset.UtcNow,
                    UserId = _user.Id,
                    FriendId = _friend.Id
                }, CancellationToken.None);
            }
        }
    }
}