using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Paging;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.DataProviders
{
    public class GroupUserDataProviderTests : ServiceFixture
    {
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserDataProviderTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _groupUserDataProvider = Container.Resolve<IGroupUserDataProvider>();
        }

        [Fact]
        public async Task Should_Add_User_To_Group()
        {
            GroupUser addSpikeToGroup = new GroupUser
            {
                Id = "0",
                UserId = Spike.Id,
                GroupId = TomAndJerryAndTykeGroup.Id
            };
            await _groupUserDataProvider.AddAsync(addSpikeToGroup);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(Spike.Id, TomAndJerryAndTykeGroup.Id);
            groupUser.ShouldNotBeNull();
            groupUser.UserId.ShouldBe(Spike.Id);
            groupUser.GroupId.ShouldBe(TomAndJerryAndTykeGroup.Id);
            groupUser.LastReadTime.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Not_Add_User_To_Group_He_Is_Already_In()
        {
            GroupUser addSpikeToGroup = new GroupUser
            {
                Id = "1",
                UserId = Spike.Id,
                GroupId = TomAndJerryAndTykeGroup.Id
            };
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await _groupUserDataProvider.AddAsync(addSpikeToGroup));
        }

        [Fact]
        public async Task Should_Remove_User_From_Group()
        {
            GroupUser removeTomFromGroup = new GroupUser
            {
                Id = TomInTomAndJerryAndTyke.Id,
                UserId = TomInTomAndJerryAndTyke.UserId,
                GroupId = TomInTomAndJerryAndTyke.GroupId
            };
            await _groupUserDataProvider.RemoveAsync(removeTomFromGroup);
            GroupUser groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(TomInTomAndJerryAndTyke.UserId,
                TomInTomAndJerryAndTyke.GroupId);
            groupUser.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Removing_None_Exist_Group()
        {
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserDataProvider.RemoveAsync(new() {Id = "0"}));
        }

        [Fact]
        public async Task Should_Get_GroupUsers_By_User_Id()
        {
            IEnumerable<GroupUser> tomGroupUsers = await _groupUserDataProvider.GetByUserIdAsync(Tom.Id);
            tomGroupUsers.Count().ShouldBe(2);
            tomGroupUsers.SingleOrDefault(o => o.GroupId == TomAndJerryGroup.Id).ShouldNotBeNull();
            tomGroupUsers.SingleOrDefault(o => o.GroupId == TomAndJerryAndTykeGroup.Id).ShouldNotBeNull();
        }


        [Fact]
        public async Task Should_Not_Get_GroupUsers_By_None_Exist_User_Id()
        {
            IEnumerable<GroupUser> noneExistGroupUsers = await _groupUserDataProvider.GetByUserIdAsync("0");
            noneExistGroupUsers.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_GroupUsers_By_Group_Id()
        {
            IEnumerable<GroupUser> tomGroupUsers = await _groupUserDataProvider.GetByGroupIdAsync(TomAndJerryGroup.Id);
            tomGroupUsers.Count().ShouldBe(2);
            tomGroupUsers.SingleOrDefault(o => o.UserId == Tom.Id).ShouldNotBeNull();
            tomGroupUsers.SingleOrDefault(o => o.UserId == Jerry.Id).ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Not_Get_GroupUsers_By_None_Exist_Group_Id()
        {
            IEnumerable<GroupUser> noneExistGroupUsers = await _groupUserDataProvider.GetByGroupIdAsync("0");
            noneExistGroupUsers.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_GroupUsers_By_User_And_Group_Id()
        {
            GroupUser tomInTomAndJerryGroupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(Tom.Id, TomAndJerryGroup.Id);
            tomInTomAndJerryGroupUser.ShouldNotBeNull();
            tomInTomAndJerryGroupUser.UserId.ShouldBe(Tom.Id);
            tomInTomAndJerryGroupUser.GroupId.ShouldBe(TomAndJerryGroup.Id);
        }
        
        [Fact]
        public async Task Should_Not_Get_GroupUsers_By_Incorrect_User_And_Group_Id()
        {
            GroupUser tomInTomAndJerryGroupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(Spike.Id, TomAndJerryGroup.Id);
            tomInTomAndJerryGroupUser.ShouldBeNull();
        }
        
        [Fact]
        public async Task Should_Set_Message_Read_To_Proper_Time()
        {
            await _groupUserDataProvider.SetMessageReadAsync(Tom.Id, TomAndJerryGroup.Id, BaseTime);
            GroupUser tomInTomAndJerryGroupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(Tom.Id, TomAndJerryGroup.Id);
            tomInTomAndJerryGroupUser.LastReadTime.ShouldBe(BaseTime);
        }
        
        [Fact]
        public async Task Should_Not_Set_Incorrect_Message_Read()
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(async () =>
                await _groupUserDataProvider.SetMessageReadAsync(Spike.Id, TomAndJerryGroup.Id, BaseTime));
        }
    }
}