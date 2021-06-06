using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Event;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class GroupUserServiceTests : ServiceFixture
    {
        private readonly IGroupUserService _groupUserService;

        public GroupUserServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _groupUserService = Container.Resolve<IGroupUserService>();
        }

        [Fact]
        public async Task Should_Add_User_To_Group()
        {
            AddUserToGroupCommand addUserToGroupCommand = new()
            {
                UserId = Tyke.Id,
                GroupId = TomAndJerryGroup.Id
            };
            UserAddedToGroupEvent addUserToGroupEvent =
                await _groupUserService.AddUserToGroupAsync(addUserToGroupCommand);
            GroupUser groupUser = await Repository.SingleOrDefaultAsync<GroupUser>(o => o.Id == addUserToGroupEvent.Id);
            groupUser.ShouldNotBeNull();
            groupUser.UserId.ShouldBe(Tyke.Id);
            groupUser.GroupId.ShouldBe(TomAndJerryGroup.Id);
            groupUser.LastReadTime.ShouldBeNull();
            addUserToGroupEvent.Id.ShouldBe(groupUser.Id);
            addUserToGroupEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Add_User_To_Group_When_They_Are_Already_In()
        {
            AddUserToGroupCommand addUserToGroupCommand = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.AddUserToGroupAsync(addUserToGroupCommand));
        }

        [Fact]
        public async Task Should_Not_Add_User_To_Group_When_User_Dose_Not_Exist()
        {
            AddUserToGroupCommand addUserToGroupCommand = new()
            {
                UserId = "0",
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.AddUserToGroupAsync(addUserToGroupCommand));
        }

        [Fact]
        public async Task Should_Not_Add_User_To_Group_When_Group_Dose_Not_Exist()
        {
            AddUserToGroupCommand addUserToGroupCommand = new()
            {
                UserId = Tom.Id,
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.AddUserToGroupAsync(addUserToGroupCommand));
        }

        [Fact]
        public async Task Should_Remove_User_From_Group()
        {
            RemoveUserFromGroupCommand removeUserFromGroupCommand = new()
            {
                UserId = Tom.Id,
                GroupId = TomAndJerryGroup.Id
            };
            UserRemovedFromGroupEvent userRemovedFromGroupEvent =
                await _groupUserService.RemoveUserFromGroupAsync(removeUserFromGroupCommand);
            GroupUser groupUser =
                await Repository.SingleOrDefaultAsync<GroupUser>(o => o.Id == userRemovedFromGroupEvent.Id);
            groupUser.ShouldBeNull();
            userRemovedFromGroupEvent.Status.ShouldBe(EventStatus.Success);
        }
        
        [Fact]
        public async Task Should_Not_Remove_User_From_Group_When_They_Are_Not_In()
        {
            RemoveUserFromGroupCommand removeUserFromGroupCommand = new()
            {
                UserId = Spike.Id,
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.RemoveUserFromGroupAsync(removeUserFromGroupCommand));
        }

        [Fact]
        public async Task Should_Not_Remove_User_From_Group_When_User_Dose_Not_Exist()
        {
            RemoveUserFromGroupCommand removeUserFromGroupCommand = new()
            {
                UserId = "0",
                GroupId = TomAndJerryGroup.Id
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.RemoveUserFromGroupAsync(removeUserFromGroupCommand));
        }

        [Fact]
        public async Task Should_Not_Remove_User_From_Group_When_Group_Dose_Not_Exist()
        {
            RemoveUserFromGroupCommand removeUserFromGroupCommand = new()
            {
                UserId = Tom.Id,
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.RemoveUserFromGroupAsync(removeUserFromGroupCommand));
        }
        
        [Fact]
        public async Task Should_Get_Group_Member_Ids_Of_Group()
        {
            GetGroupMembersRequest getGroupMembersRequest = new()
            {
                GroupId = TomAndJerryAndTykeGroup.Id
            };
            GetGroupMembersResponse getGroupMembersResponse =
                await _groupUserService.GetGroupMemberIdsAsync(getGroupMembersRequest);
            getGroupMembersResponse.MemberIds.Count().ShouldBe(3);
            getGroupMembersResponse.MemberIds.SingleOrDefault(o => o == Tom.Id).ShouldNotBeNull();
            getGroupMembersResponse.MemberIds.SingleOrDefault(o => o == Jerry.Id).ShouldNotBeNull();
            getGroupMembersResponse.MemberIds.SingleOrDefault(o => o == Tyke.Id).ShouldNotBeNull();
        }
        
        [Fact]
        public async Task Should_Not_Get_Group_Member_Ids_When_Group_Dose_Not_Exist()
        {
            GetGroupMembersRequest getGroupMembersRequest = new()
            {
                GroupId = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupUserService.GetGroupMemberIdsAsync(getGroupMembersRequest));
        }
    }
}