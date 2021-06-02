using System.Threading.Tasks;
using Autofac;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Users;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Users;
using SugarChat.Shared.Dtos;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.Services
{
    public class UserServiceTests : ServiceFixture
    {
        private readonly IUserService _userService;

        public UserServiceTests(DatabaseFixture dbFixture) : base(dbFixture)
        {
            _userService = Container.Resolve<IUserService>();
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
            user.DisplayName.ShouldBe(addUserCommand.Id);
            addUserEvent.Status.ShouldBe(EventStatus.Success);
        }

       
    }
}