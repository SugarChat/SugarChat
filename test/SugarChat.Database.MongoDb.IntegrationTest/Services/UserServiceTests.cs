using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class UserServiceTests : ServiceFixture
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _userService = Container.Resolve<IUserService>();
            _mapper = Container.Resolve<IMapper>();
        }

        [Fact]
        public async Task Should_Add_User()
        {
            AddUserCommand addUserCommand = new()
            {
                Id = "0",
                DisplayName = "Micky"
            };
            AddUserEvent addUserEvent =
                await _userService.AddUserAsync(addUserCommand);
            User user = await Repository.SingleOrDefaultAsync<User>(o => o.Id == addUserCommand.Id);
            user.ShouldNotBeNull();
            user.Id.ShouldBe(addUserCommand.Id);
            user.DisplayName.ShouldBe(addUserCommand.DisplayName);
            addUserEvent.Id.ShouldBe(addUserCommand.Id);
            addUserEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Add_User_Who_Exists()
        {
            AddUserCommand addUserCommand = new()
            {
                Id = "1",
                DisplayName = "Tom"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () => await _userService.AddUserAsync(addUserCommand));
        }

        [Fact]
        public async Task Should_Update_User()
        {
            UpdateUserCommand updateUserCommand = new()
            {
                Id = "1",
                DisplayName = "Micky"
            };
            UpdateUserEvent updateUserEvent =
                await _userService.UpdateUserAsync(updateUserCommand);
            User user = await Repository.SingleOrDefaultAsync<User>(o => o.Id == updateUserCommand.Id);
            user.ShouldNotBeNull();
            user.Id.ShouldBe(updateUserCommand.Id);
            user.DisplayName.ShouldBe(updateUserCommand.DisplayName);
            updateUserEvent.Id.ShouldBe(updateUserCommand.Id);
            updateUserEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Update_User_Who_Dose_Not_Exists()
        {
            UpdateUserCommand updateUserCommand = new()
            {
                Id = "0",
                DisplayName = "Tom"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _userService.UpdateUserAsync(updateUserCommand));
        }

        [Fact]
        public async Task Should_Remove_User()
        {
            RemoveUserCommand removeUserCommand = new()
            {
                Id = Tom.Id
            };
            RemoveUserEvent removeUserEvent =
                await _userService.RemoveUserAsync(removeUserCommand);
            User user = await Repository.SingleOrDefaultAsync<User>(o => o.Id == removeUserEvent.Id);
            user.ShouldBeNull();
            removeUserEvent.Id.ShouldBe(Tom.Id);
            removeUserEvent.Status.ShouldBe(EventStatus.Success);
        }

        [Fact]
        public async Task Should_Not_Remove_User_Who_Dose_Not_Exists()
        {
            RemoveUserCommand removeUserCommand = new()
            {
                Id = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _userService.RemoveUserAsync(removeUserCommand));
        }
        
        [Fact]
        public async Task Should_Get_User()
        {
            GetUserRequest getUserRequest = new()
            {
                Id = Tom.Id
            };
            GetUserResponse getUserResponse =
                await _userService.GetUserAsync(getUserRequest);
            getUserResponse.User.ShouldNotBeNull();
            getUserResponse.User.ShouldBeEquivalentTo(_mapper.Map<UserDto>(Tom));
        }
        
        [Fact]
        public async Task Should_Not_Get_User_Who_Dose_Not_Exists()
        {
            GetUserRequest getUserRequest = new()
            {
                Id = "0"
            };
            await Assert.ThrowsAnyAsync<BusinessException>(async () =>
                await _userService.GetUserAsync(getUserRequest));
        }
        
        [Fact]
        public async Task Should_Get_Current_User()
        {
            GetCurrentUserRequest getCurrentUserRequest = new()
            {
            };
            GetCurrentUserResponse getCurrentUserResponse =
                await _userService.GetCurrentUserAsync(getCurrentUserRequest);
            getCurrentUserResponse.User.ShouldNotBeNull();
        }
    }
}