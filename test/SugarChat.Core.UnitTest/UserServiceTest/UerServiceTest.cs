using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using SugarChat.Core.Domain;
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

        

        public UerServiceTest()
        {
            _repository = Substitute.For<IRepository>();
            _userService = new UserService(_repository);
            _user = new User
            {
                Id = _userId,
                AvatarUrl = "https://test.com",
                CreatedBy = "Admin",
                CreatedDate = _createDateTime,
                CustomProperties = new Dictionary<string, string>(),
                DisplayName = "Tom",
                
                



            };
        }
        
        [Fact]
        public Task Should_Always_Be_Ture()
        {
            User user = new User
            {
                
            }
            _userService.AddAsync()
        }
    }
}