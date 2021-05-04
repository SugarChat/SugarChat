using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Users;
using Xunit;

namespace SugarChat.Core.UnitTest.UserServiceTest
{
    public class UerServiceTest
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
            _repository = Substitute.For<IRepository>();
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
            await _userService.AddAsync(_user, CancellationToken.None);
            await _repository.Received().AddAsync(_user, CancellationToken.None);
        }
        
        [Fact]
        public async Task Should_Not_Add_Null_User()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async ()=>await _userService.AddAsync(null, CancellationToken.None));
            await _repository.DidNotReceive().AddAsync(_user, CancellationToken.None);
        }
        
        [Fact]
        public async Task Should_Reject_To_Add_Existed_User()
        {
            _repository.AnyAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs(true);
            await Assert.ThrowsAsync<BusinessException>(async ()=>await _userService.AddAsync(_user, CancellationToken.None));
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
            _repository.SingleOrDefaultAsync<User>(o => o.Id == _user.Id).ReturnsForAnyArgs((User)null);
            await _userService.DeleteAsync(_user.Id, CancellationToken.None);
            await _repository.DidNotReceive().RemoveAsync(_user, CancellationToken.None);
        }
    }
}