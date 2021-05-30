using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Users;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class UserDataProviderTests : ServiceFixture
    {
        private readonly IUserDataProvider _userDataProvider;

        public UserDataProviderTests()
        {
            _userDataProvider = new UserDataProvider(Repository);
        }

        [Fact]
        public async Task Should_Get_Exist_User_By_Id()
        {
            User user = await _userDataProvider.GetByIdAsync("1");
            user.Id.ShouldBe("1");
            user.DisplayName.ShouldBe("Tom");
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_User_By_Id()
        {
            User user = await _userDataProvider.GetByIdAsync("0");
            user.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Get_Exist_Users_By_Ids()
        {
            var users = (await _userDataProvider.GetRangeByIdAsync(new[] {"1", "2"})).OrderBy(o => o.Id).ToArray();
            users.Length.ShouldBe(2);
            users[0].Id.ShouldBe("1");
            users[0].DisplayName.ShouldBe("Tom");
            users[1].Id.ShouldBe("2");
            users[1].DisplayName.ShouldBe("Jerry");
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_Users_By_Ids()
        {
            var users = (await _userDataProvider.GetRangeByIdAsync(new[] {"1", "2", "0"})).OrderBy(o => o.Id).ToArray();
            users.Length.ShouldBe(2);
            users[0].Id.ShouldBe("1");
            users[0].DisplayName.ShouldBe("Tom");
            users[1].Id.ShouldBe("2");
            users[1].DisplayName.ShouldBe("Jerry");
        }

        [Fact]
        public async Task Should_Add_None_Exist_User()
        {
            await _userDataProvider.AddAsync(new() {Id = "0", DisplayName = "Tweety"});
            User user = await _userDataProvider.GetByIdAsync("0");
            user.ShouldNotBeNull();
            user.Id.ShouldBe("0");
            user.DisplayName.ShouldBe("Tweety");
        }
        
        [Fact]
        public async Task Should_Not_Add_Exist_User()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _userDataProvider.AddAsync(new() {Id = "1", DisplayName = "Tweety"}));

        }
        
        [Fact]
        public async Task Should_Update_Exist_User()
        {
            await _userDataProvider.UpdateAsync(new() {Id = "1", DisplayName = "Tweety"});
            User user = await _userDataProvider.GetByIdAsync("1");
            user.DisplayName.ShouldBe("Tweety");
        }
        
        [Fact(Skip = "The IRepo is fixing the bug")]
        public async Task Should_Throw_Exception_On_Update_None_Exist_User()
        {
            await _userDataProvider.UpdateAsync(new() {Id = "0", DisplayName = "Tweety"});
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _userDataProvider.UpdateAsync(new() {Id = "0", DisplayName = "Tyke"}));
        }
        
        [Fact]
        public async Task Should_Remove_Exist_User()
        {
            await _userDataProvider.RemoveAsync(new() {Id = "1"});
            User user = await _userDataProvider.GetByIdAsync("1");
            user.ShouldBeNull();
        }
        
                
        [Fact(Skip = "The IRepo is fixing the bug")]
        public async Task Should_Throw_Exception_On_Removing_None_Exist_User()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _userDataProvider.RemoveAsync(new() {Id = "0"}));
        }
    }
}