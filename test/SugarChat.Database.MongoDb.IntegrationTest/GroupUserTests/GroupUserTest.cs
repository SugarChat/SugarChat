using System;
using System.Threading.Tasks;
using Shouldly;
using SugarChat.Core.Domain;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest.GroupUserTests
{
    public class GroupUserTest : ServiceFixture
    {
        private GroupUser _groupUser;

        public GroupUserTest(DatabaseFixture dbFixture) : base(dbFixture)
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
                await Repository.AddAsync(_groupUser);
                GroupUser groupUser = await Repository.SingleAsync<GroupUser>(o => o.Id == _groupUser.Id);
                groupUser.ShouldBeEquivalentTo(_groupUser);
            }
            finally
            {
                await Repository.RemoveAsync(_groupUser);
            }
        }
    }
}