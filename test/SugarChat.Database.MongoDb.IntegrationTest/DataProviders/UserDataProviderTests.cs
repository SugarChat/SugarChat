using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Users;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class UserDataProviderTests : ServiceFixture
    {
        private readonly IUserDataProvider _userDataProvider;

        public UserDataProviderTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _userDataProvider = Container.Resolve<IUserDataProvider>();
        }

        [Fact]
        public async Task Should_Get_Exist_User_By_Id()
        {
            User user = await _userDataProvider.GetByIdAsync(Tom.Id);
            user.Id.ShouldBe(Tom.Id);
            user.DisplayName.ShouldBe(Tom.DisplayName);
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
            var users = (await _userDataProvider.GetRangeByIdAsync(new[] {Tom.Id, Jerry.Id})).ToArray();
            users.Length.ShouldBe(2);
            var tom = users.SingleOrDefault(o => o.Id == Tom.Id);
            tom.ShouldNotBeNull();
            tom.DisplayName.ShouldBe(Tom.DisplayName);
            var jerry = users.SingleOrDefault(o => o.Id == Jerry.Id);
            jerry.ShouldNotBeNull();
            jerry.DisplayName.ShouldBe(Jerry.DisplayName);
        }
        
        [Fact]
        public async Task Should_Not_Get_None_Exist_Users_By_Ids()
        {
            var users = (await _userDataProvider.GetRangeByIdAsync(new[] {Tom.Id, Jerry.Id, "0"})).ToArray();
            users.Length.ShouldBe(2);
            var tom = users.SingleOrDefault(o => o.Id == Tom.Id);
            tom.ShouldNotBeNull();
            tom.DisplayName.ShouldBe(Tom.DisplayName);
            var jerry = users.SingleOrDefault(o => o.Id == Jerry.Id);
            jerry.ShouldNotBeNull();
            jerry.DisplayName.ShouldBe(Jerry.DisplayName);
        }
        
        [Fact]
        public async Task Should_Add_None_Exist_User()
        {
            User tweety = new() {Id = "0", DisplayName = "Tweety"};
            await _userDataProvider.AddAsync(tweety);
            User user = await _userDataProvider.GetByIdAsync(tweety.Id);
            user.ShouldNotBeNull();
            user.Id.ShouldBe(tweety.Id);
            user.DisplayName.ShouldBe(tweety.DisplayName);
        }
        
        [Fact]
        public async Task Should_Not_Add_Exist_User()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _userDataProvider.AddAsync(new() {Id = Tom.Id, DisplayName = "Tweety"}));
        }
        
        [Fact]
        public async Task Should_Update_Exist_User()
        {
            await _userDataProvider.UpdateAsync(new() {Id = Tom.Id, DisplayName = "Tweety"});
            User user = await _userDataProvider.GetByIdAsync(Tom.Id);
            user.DisplayName.ShouldBe("Tweety");
        }
        
        [Fact]
        public async Task Should_Throw_Exception_On_Update_None_Exist_User()
        {
            User tweety = new() {Id = "0", DisplayName = "Tweety"};
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _userDataProvider.UpdateAsync(tweety));
        }
        
        [Fact]
        public async Task Should_Remove_Exist_User()
        {
            await _userDataProvider.RemoveAsync(Tom);
            User user = await _userDataProvider.GetByIdAsync(Tom.Id);
            user.ShouldBeNull();
        }
        
        
        [Fact]
        public async Task Should_Throw_Exception_On_Removing_None_Exist_User()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _userDataProvider.RemoveAsync(new() {Id = "0"}));
        }
        
        
    }
}