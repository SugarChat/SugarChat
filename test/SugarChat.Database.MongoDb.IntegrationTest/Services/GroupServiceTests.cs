using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class GroupServiceTests : ServiceFixture
    {
        private readonly IGroupService _groupService;

        public GroupServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _groupService = Container.Resolve<IGroupService>();
        }

        [Fact]
        public async Task Should_Add_Group()
        {
            AddGroupCommand addGroupCommand = new()
            {
                Id = "0",
                UserId = "1",
                Description = "Friend group of Tom and Tyke"
            };
            GroupAddedEvent addGroupEvent =
                await _groupService.AddGroupAsync(addGroupCommand);
            Group group = await Repository.SingleOrDefaultAsync<Group>(o => o.Id == addGroupCommand.Id);
            group.ShouldNotBeNull();
            group.Id.ShouldBe(addGroupCommand.Id);
            group.Description.ShouldBe(addGroupCommand.Description);
            addGroupEvent.Id.ShouldBe(addGroupCommand.Id);
            addGroupEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Add_Group_When_Exist()
        {
            AddGroupCommand addGroupCommand = new()
            {
                Id = "1",
                Description = "Friend group of Tom and Tyke"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupService.AddGroupAsync(addGroupCommand));
        }

        [Fact]
        public async Task Should_Get_Groups_Of_User()
        {
            GetGroupsOfUserRequest getGroupsOfUserRequest = new()
            {
                Id = "1",
                PageSettings = new PageSettings {PageNum = 1}
            };
            GetGroupsOfUserResponse getGroupsOfUserResponse =
                await _groupService.GetGroupsOfUserAsync(getGroupsOfUserRequest);
            getGroupsOfUserResponse.Groups.Total.ShouldBe(2);
            GroupDto group = getGroupsOfUserResponse.Groups.Result.SingleOrDefault(o => o.Id == TomAndJerryGroup.Id);
            group.ShouldNotBeNull();
            group.Id.ShouldBe(getGroupsOfUserRequest.Id);
            group.Description.ShouldBe(TomAndJerryGroup.Description);
        }

        [Fact]
        public async Task Should_Not_Get_Group_When_User_Dose_Not_Exist()
        {
            GetGroupsOfUserRequest getGroupsOfUserRequest = new()
            {
                Id = "0",
                PageSettings = new PageSettings {PageNum = 1}
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupService.GetGroupsOfUserAsync(getGroupsOfUserRequest));
        }
        
        [Fact]
        public async Task Should_Remove_Group()
        {
            RemoveGroupCommand removeGroupCommand = new()
            {
                Id = "1"
            };
            GroupRemovedEvent removeGroupEvent =
                await _groupService.RemoveGroupAsync(removeGroupCommand);
            Group group = await Repository.SingleOrDefaultAsync<Group>(o => o.Id == removeGroupCommand.Id);
            group.ShouldBeNull();
            removeGroupEvent.Id.ShouldBe(TomAndJerryGroup.Id);
            removeGroupEvent.Status.ShouldBe(EventStatus.Success);
        }
        
        [Fact]
        public async Task Should_Not_Remove_Group_Not_Exist()
        {
            RemoveGroupCommand removeGroupCommand = new()
            {
                Id = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _groupService.RemoveGroupAsync(removeGroupCommand));
        }
    }
}