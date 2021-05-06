using Microsoft.Extensions.Configuration;
using Shouldly;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Settings;
using SugarChat.Data.MongoDb;
using SugarChat.Core.Domain;
using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class MongoDbRepositoryTest : TestBase, IDisposable
    {
        readonly IRepository repository;
        readonly Group _group;
        public MongoDbRepositoryTest()
        {
            MongoDbSettings settings = new MongoDbSettings();
            _configuration.GetSection("MongoDb")
                          .Bind(settings);
            repository = new MongoDbRepository(settings);
            _group = new Group()
            {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = Guid.NewGuid().ToString(),
                LastModifyDate = DateTime.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "TestGroup"
            };
            repository.AddAsync(_group).Wait();
        }

        [Fact]
        public async Task Should_Insert_One_Group_And_Remove_It()
        {
            var newGroup = new Group()
            {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = Guid.NewGuid().ToString(),
                LastModifyDate = DateTime.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "NewTestGroup"
            };
            await repository.AddAsync(newGroup, default(CancellationToken));
            var firstGroup = await repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
            firstGroup.ShouldNotBeNull();
            await repository.RemoveAsync(newGroup);
            var deleted = await repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
            deleted.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Insert_Many_Groups_And_Remove_Them()
        {
            List<Group> groups = new List<Group> {
                new Group {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = Guid.NewGuid().ToString(),
                LastModifyDate = DateTime.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "NewTestGroup1"
            },
              new Group   {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = Guid.NewGuid().ToString(),
                LastModifyDate = DateTime.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "NewTestGroup2"
            }
        };
            await repository.AddRangeAsync(groups, default(CancellationToken));
            var ids = groups.Select(e => e.Id).ToList();
            var groupList = await repository.ToListAsync<Group>(e => ids.Contains(e.Id));
            groupList.Count.ShouldBe(groups.Count);
            await repository.RemoveRangeAsync(groupList);
            var deleted = await repository.ToListAsync<Group>(e => ids.Contains(e.Id));
            deleted.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_FirstOrDefault_Group_Be_Not_Null()
        {
            var group = await repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
            group.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_FirstOrDefault_Group_Be_Null()
        {
            var group = await repository.FirstOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            group.ShouldBeNull();
        }

        [Fact]
        public async Task Should_List_Group_Be_Empty()
        {
            var list = await repository.ToListAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            list.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_List_Group_Not_Be_Empty()
        {
            var list = await repository.ToListAsync<Group>(e => e.Id == _group.Id);
            list.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Should_Any_Group_Be_False()
        {
            var any = await repository.AnyAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            any.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Any_Group_Be_True()
        {
            var any = await repository.AnyAsync<Group>(e => e.Id == _group.Id);
            any.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Count_Group_Great_Than_0()
        {
            var any = await repository.CountAsync<Group>(e => e.Id == _group.Id);
            any.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Count_Group_Equivalent_0()
        {
            var any = await repository.CountAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            any.ShouldBeEquivalentTo(0);
        }


        [Fact]
        public Task Should_Single_Group_Throw_Exception()
        {
            return Should.ThrowAsync<Exception>(() =>
             repository.SingleAsync<Group>(e => e.Id == Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task Should_Single_Group_Not_Throw_Exception()
        {
            var single = await repository.SingleAsync<Group>(e => e.Id == _group.Id);
            single.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_SingleOrDefault_Group_Be_Default()
        {
            var single = await repository.SingleOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            single.ShouldBe(default);
        }

        [Fact]
        public async Task Should_SingleOrDefault_Group_Not_Be_Default()
        {
            var single = await repository.SingleOrDefaultAsync<Group>(e => e.Id == _group.Id);
            single.ShouldNotBeNull();
        }

        public void Dispose()
        {
            repository.RemoveAsync(_group).Wait();
        }
    }
}
