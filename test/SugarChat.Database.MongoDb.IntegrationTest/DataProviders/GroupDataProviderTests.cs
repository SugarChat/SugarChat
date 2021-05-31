using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Groups;
using SugarChat.Shared.Paging;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class GroupDataProviderTests : ServiceFixture
    {
        private readonly IGroupDataProvider _groupDataProvider;

        public GroupDataProviderTests()
        {
            _groupDataProvider = new GroupDataProvider(Repository);
        }

        [Fact]
        public async Task Should_Get_Exist_Group_By_Id()
        {
            Group group = await _groupDataProvider.GetByIdAsync(TomAndJerryGroup.Id);
            group.Id.ShouldBe(TomAndJerryGroup.Id);
            group.Description.ShouldBe(TomAndJerryGroup.Description);
        }
        
        [Fact]
        public async Task Should_Not_Get_None_Exist_Group_By_Id()
        {
            Group group = await _groupDataProvider.GetByIdAsync("0");
            group.ShouldBeNull();
        }
        
        [Fact]
        public async Task Should_Get_Exist_Groups_By_Ids()
        {
            PageSettings pageSettings = new PageSettings{PageNum = 1};
            var groups = await _groupDataProvider.GetByIdsAsync(new[] {TomAndJerryGroup.Id, TomAndJerryAndTykeGroup.Id},
                pageSettings);
            groups.Total.ShouldBe(await Repository.CountAsync<Group>());
            var result = groups.Result;
            result.Count().ShouldBe(2);
            var tomAndJerryGroup = result.Single(o => o.Id == TomAndJerryGroup.Id);
            tomAndJerryGroup.ShouldNotBeNull();
            tomAndJerryGroup.Description.ShouldBe(TomAndJerryGroup.Description);
            var tomAndJerryAndTykeGroup = result.Single(o => o.Id == TomAndJerryAndTykeGroup.Id);
            tomAndJerryAndTykeGroup.ShouldNotBeNull();
            tomAndJerryAndTykeGroup.Description.ShouldBe(TomAndJerryAndTykeGroup.Description);
        }

        [Fact]
        public async Task Should_Not_Get_None_Exist_Users_By_Ids()
        {
            PageSettings pageSettings = new PageSettings{PageNum = 1};
            var groups = await _groupDataProvider.GetByIdsAsync(new[] {TomAndJerryGroup.Id, TomAndJerryAndTykeGroup.Id, "0"},pageSettings);
            groups.Total.ShouldBe(await Repository.CountAsync<Group>());
            var result = groups.Result;
            result.Count().ShouldBe(2);
            var tomAndJerryGroup = result.Single(o => o.Id == TomAndJerryGroup.Id);
            tomAndJerryGroup.ShouldNotBeNull();
            tomAndJerryGroup.Description.ShouldBe(TomAndJerryGroup.Description);
            var tomAndJerryAndTykeGroup = result.Single(o => o.Id == TomAndJerryAndTykeGroup.Id);
            tomAndJerryAndTykeGroup.ShouldNotBeNull();
            tomAndJerryAndTykeGroup.Description.ShouldBe(TomAndJerryAndTykeGroup.Description);
        }

        [Fact]
        public async Task Should_Add_None_Exist_Group()
        {
            Group tomAndJerryAndSpikeGroup = new() {Id = "0", Description = "tomAndJerryAndSpikeGroup"};
            await _groupDataProvider.AddAsync(tomAndJerryAndSpikeGroup);
            Group group = await _groupDataProvider.GetByIdAsync(tomAndJerryAndSpikeGroup.Id);
            group.ShouldNotBeNull();
            group.Id.ShouldBe(tomAndJerryAndSpikeGroup.Id);
            group.Description.ShouldBe(tomAndJerryAndSpikeGroup.Description);
        }

        [Fact]
        public async Task Should_Not_Add_Exist_Group()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _groupDataProvider.AddAsync(new() {Id = TomAndJerryGroup.Id, Description = "tomAndJerryAndSpikeGroup"}));
        }

        [Fact]
        public async Task Should_Update_Exist_Group()
        {
            await _groupDataProvider.UpdateAsync(new() {Id = TomAndJerryGroup.Id, Description = "TomAndJerryGroup"});
            Group group = await _groupDataProvider.GetByIdAsync(TomAndJerryGroup.Id);
            group.Description.ShouldBe("TomAndJerryGroup");
        }

        // [Fact(Skip = "The IRepo is fixing the bug")]
        // public async Task Should_Throw_Exception_On_Update_None_Exist_Group()
        // {
        //     Group tomAndJerryAndSpikeGroup = new() {Id = "0", Description = "tomAndJerryAndSpikeGroup"};
        //     await Assert.ThrowsAnyAsync<Exception>(async () =>
        //         await _groupDataProvider.UpdateAsync(tomAndJerryAndSpikeGroup));
        // }

        [Fact]
        public async Task Should_Remove_Exist_Group()
        {
            await _groupDataProvider.RemoveAsync(TomAndJerryGroup);
            Group group = await _groupDataProvider.GetByIdAsync(TomAndJerryGroup.Id);
            group.ShouldBeNull();
        }


        // [Fact(Skip = "The IRepo is fixing the bug")]
        // public async Task Should_Throw_Exception_On_Removing_None_Exist_Group()
        // {
        //     await Assert.ThrowsAnyAsync<Exception>(async () =>
        //         await _groupDataProvider.RemoveAsync(new() {Id = "0"}));
        // }
    }
}