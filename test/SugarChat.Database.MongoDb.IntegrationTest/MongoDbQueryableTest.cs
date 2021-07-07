using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb.Settings;
using SugarChat.Data.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using SugarChat.Core.Domain;
using Shouldly;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class MongoDbQueryableTest : TestBase
    {
        Group _group;
        public MongoDbQueryableTest(DatabaseFixture dbFixture) : base(dbFixture)
        {
            InsertOneGroup().Wait();
        }

        private Task InsertOneGroup()
        {
            _group = new Group()
            {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.UtcNow,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = Guid.NewGuid().ToString(),
                LastModifyDate = DateTime.UtcNow,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "TestGroup"
            };
            return Repository.AddAsync(_group);
        }

        [Fact]
        public async Task Should_Select_Specified_Property()
        {
            var group = await Repository
                .Query<Group>()
                .Select(e => new
                {
                    e.Id
                })
                .SingleAsync(e => e.Id == _group.Id);
            var type = group.GetType();
            var props = type.GetProperties();
            props.Length.ShouldBe(1);
            props[0].Name.ShouldBe("Id");
        }

        [Fact]
        public async Task Should_Query_With_ListAsync()
        {
            var group = await Repository
                     .Query<Group>()
                     .Where(e => e.Id == _group.Id)
                     .ToListAsync();
            group.Single().Id.ShouldBe(_group.Id);
        }

        [Fact]
        public async Task Should_Query_With_FirstOrDefaultAsync()
        {
            var group = await Repository
                     .Query<Group>()
                     .Where(e => e.Id == _group.Id)
                     .FirstOrDefaultAsync();
            group.Id.ShouldBe(_group.Id);
        }
    }
}
