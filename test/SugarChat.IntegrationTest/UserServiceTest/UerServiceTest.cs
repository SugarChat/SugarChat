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
            (await _repository.SingleOrDefaultAsync<User>(o => o.Id == _userId)).ShouldBe(null);
            await _userService.AddAsync(_user, CancellationToken.None);
            (await _repository.SingleAsync<User>(o => o.Id == _userId)).ShouldNotBe(null);
        }

        [Fact]
        public async Task Should_Not_Add_Null_User()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _userService.AddAsync(null, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(_user, CancellationToken.None);
        }

        [Fact]
        public async Task Should_Reject_To_Add_Existed_User()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddAsync(_user, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(_user, CancellationToken.None);
        }

        [Fact]
        public async Task Should_Delete_Exist_User()
        {
            _repository.SingleOrDefaultAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(_user);
            await _userService.DeleteAsync(_user.Id, CancellationToken.None);
            await _repository.Received().RemoveAsync(_user, CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Delete_None_Exist_User()
        {
            _repository.SingleOrDefaultAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs((User) null);
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.DeleteAsync(_user.Id, CancellationToken.None));
            await _repository.DidNotReceive().RemoveAsync(_user, CancellationToken.None);
        }

        [Fact]
        public async Task Should_Get_Exist_User()
        {
            _repository.SingleOrDefaultAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(_user);
            await _userService.GetAsync(_user.Id);
            await _repository.Received().SingleOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>());
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_User()
        {
            _repository.SingleOrDefaultAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs((User) null);
            await Assert.ThrowsAsync<BusinessException>(async () => await _userService.GetAsync(_user.Id));
            await _repository.Received().SingleOrDefaultAsync(Arg.Any<Expression<Func<User, bool>>>());
        }

        [Fact]
        public async Task Should_Add_Friend()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            _repository.AnyAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs(false);
            await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None);
            await _repository.Received().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_User_Not_Exist()
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Friend_Not_Exist()
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Both_Not_Exist()
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddFriendAsync(_user.Id, _friendId, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Add_Self()
        {
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddFriendAsync(_userId, _userId, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Add_Friend_When_Relation_Already_Exists()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            _repository.AnyAsync<Friend>(o => o.Id == "").ReturnsForAnyArgs(true);
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.AddFriendAsync(_userId, _friendId, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Remove_Friend()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            _repository.SingleOrDefaultAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs(new Friend());
            await _userService.RemoveFriendAsync(_user.Id, _friendId);
            await _repository.Received().RemoveAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_User_Not_Exist()
        {
            _repository.SingleOrDefaultAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs(new Friend());
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.RemoveFriendAsync(_user.Id, _friendId));
            await _repository.DidNotReceive().RemoveAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Friend_Not_Exist()
        {
            _repository.SingleOrDefaultAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs(new Friend());
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.RemoveFriendAsync(_user.Id, _friendId));
            await _repository.DidNotReceive().RemoveAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Both_Not_Exist()
        {
            _repository.SingleOrDefaultAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs(new Friend());
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.RemoveFriendAsync(_user.Id, _friendId));
            await _repository.DidNotReceive().RemoveAsync(Arg.Any<Friend>(), CancellationToken.None);
        }

        [Fact]
        public async Task Should_Not_Remove_Friend_When_Relation_Dose_Not_Exists()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            _repository.AnyAsync<Friend>(o => o.Id == "").ReturnsForAnyArgs(true);
            _repository.SingleOrDefaultAsync<Friend>(o => o.Id == _user.Id).ReturnsForAnyArgs((Friend)null);
            await Assert.ThrowsAsync<BusinessException>(async () =>
                await _userService.RemoveFriendAsync(_userId, _friendId));
            await _repository.DidNotReceive().RemoveAsync(Arg.Any<Friend>(), CancellationToken.None);
        }
    }
}