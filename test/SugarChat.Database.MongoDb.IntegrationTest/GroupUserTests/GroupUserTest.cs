using System;
using System.Threading.Tasks;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.GroupUserTests
{
    public class GroupUserTest
    {
        private readonly IRepository _repository = MongoDbFactory.GetRepository();
        private GroupUser _groupUser;

        public GroupUserTest()
        {
            _groupUser = new GroupUser
            {
                Id = "SomeId",
                UserId = "SomeUserId",
                GroupId = "SomeGroupId",
                LastReadTime = new DateTimeOffset(2021, 1, 1, 0, 0, 0, new TimeSpan())
            };
        }

        [Fact]
        public async Task Should_Persist_And_Retrieve_GroupUser()
        {
            try
            {
                await _repository.AddAsync(_groupUser);
                GroupUser groupUser = await _repository.SingleAsync<GroupUser>(o => o.Id == _groupUser.Id);
                groupUser.ShouldBeEquivalentTo(_groupUser);
            }
            finally
            {
                await _repository.RemoveAsync(_groupUser);
            }
        }
    }
}