﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
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
        public async Task Should_Get_GroupUsers_By_User_Id()
        {
            IEnumerable<GroupUser> tomGroupUsers = await _groupUserDataProvider.GetByUserIdAsync(Tom.Id, null, 11);
            tomGroupUsers.Count().ShouldBe(2);
            tomGroupUsers.SingleOrDefault(o => o.GroupId == TomAndJerryGroup.Id).ShouldNotBeNull();
            tomGroupUsers.SingleOrDefault(o => o.GroupId == TomAndJerryAndTykeGroup.Id).ShouldNotBeNull();
        }


        [Fact]
        public async Task Should_Not_Get_GroupUsers_By_None_Exist_User_Id()
        {
            IEnumerable<GroupUser> noneExistGroupUsers = await _groupUserDataProvider.GetByUserIdAsync("0", null, 10);
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
    }
}