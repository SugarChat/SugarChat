using Microsoft.Extensions.Configuration;
using Shouldly;
using SugarChat.Core.IRepositories;
using SugarChat.Data.MongoDb;
using SugarChat.Core.Domain;
using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using SugarChat.Data.MongoDb.Settings;
using System.Linq;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class MongoDbRepositoryTest : TestBase, IDisposable
    {
        readonly IRepository _repository;
        readonly Group _group;
        public MongoDbRepositoryTest()
        {
            MongoDbSettings settings = new MongoDbSettings();
            _configuration.GetSection("MongoDb")
                          .Bind(settings);
            _repository = new MongoDbRepository(settings);
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
            _repository.AddAsync(_group).Wait();
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
            await _repository.AddAsync(newGroup, default(CancellationToken));
            var firstGroup = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
            firstGroup.ShouldNotBeNull();
            await _repository.RemoveAsync(newGroup);
            var deleted = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
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
            await _repository.AddRangeAsync(groups, default(CancellationToken));
            var ids = groups.Select(e => e.Id).ToList();
            var groupList = await _repository.ToListAsync<Group>(e => ids.Contains(e.Id));
            groupList.Count.ShouldBe(groups.Count);
            await _repository.RemoveRangeAsync(groupList);
            var deleted = await _repository.ToListAsync<Group>(e => ids.Contains(e.Id));
            deleted.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_FirstOrDefault_Group_Be_Not_Null()
        {
            var group = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
            group.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_FirstOrDefault_Group_Be_Null()
        {
            var group = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            group.ShouldBeNull();
        }

        [Fact]
        public async Task Should_List_Group_Be_Empty()
        {
            var list = await _repository.ToListAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            list.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_List_Group_Not_Be_Empty()
        {
            var list = await _repository.ToListAsync<Group>(e => e.Id == _group.Id);
            list.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Should_Any_Group_Be_False()
        {
            var any = await _repository.AnyAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            any.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Any_Group_Be_True()
        {
            var any = await _repository.AnyAsync<Group>(e => e.Id == _group.Id);
            any.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Count_Group_Great_Than_0()
        {
            var any = await _repository.CountAsync<Group>(e => e.Id == _group.Id);
            any.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Count_Group_Equivalent_0()
        {
            var any = await _repository.CountAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            any.ShouldBeEquivalentTo(0);
        }

        [Fact]
        public Task Should_Single_Group_Throw_Exception()
        {
            return Should.ThrowAsync<Exception>(() =>
             _repository.SingleAsync<Group>(e => e.Id == Guid.NewGuid().ToString()));
        }

        [Fact]
        public async Task Should_Single_Group_Not_Throw_Exception()
        {
            var single = await _repository.SingleAsync<Group>(e => e.Id == _group.Id);
            single.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_SingleOrDefault_Group_Be_Default()
        {
            var single = await _repository.SingleOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            single.ShouldBe(default);
        }

        [Fact]
        public async Task Should_SingleOrDefault_Group_Not_Be_Default()
        {
            var single = await _repository.SingleOrDefaultAsync<Group>(e => e.Id == _group.Id);
            single.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Update_Group_Name()
        {
            var firstGroup = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
            firstGroup.ShouldNotBeNull();
            string updatedName = "UpdatedGroupName";
            firstGroup.Name = updatedName;
            await _repository.UpdateAsync(firstGroup);
            var updatedGroup = await _repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
            Assert.Equal(updatedGroup.Name, updatedName);
            Assert.Equal(updatedGroup.AvatarUrl, firstGroup.AvatarUrl);
            Assert.Equal(updatedGroup.CreatedBy, firstGroup.CreatedBy);
            Assert.Equal(updatedGroup.CreatedDate, firstGroup.CreatedDate);
            Assert.Equal(updatedGroup.CustomProperties, firstGroup.CustomProperties);
            Assert.Equal(updatedGroup.Description, firstGroup.Description);
            Assert.Equal(updatedGroup.LastModifyBy, firstGroup.LastModifyBy);
            Assert.Equal(updatedGroup.LastModifyDate, firstGroup.LastModifyDate);
        }

        [Fact]
        public async Task Should_Update_Group_Name_By_Range()
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
            await _repository.AddRangeAsync(groups, default(CancellationToken));
            groups[0].Name = "UpdatedTestGroup1";
            groups[1].Name = "UpdatedTestGroup2";
            await _repository.UpdateRangeAsync(groups);
            var ids = groups.Select(e => e.Id).ToList();
            var groupList = await _repository.ToListAsync<Group>(e => ids.Contains(e.Id));
            groupList.Count.ShouldBe(groups.Count);
            foreach (var updatedGroup in groupList)
            {
                var group = groups.FirstOrDefault(e => e.Id == updatedGroup.Id);
                Assert.Equal(updatedGroup.Name, group.Name);
                Assert.Equal(updatedGroup.AvatarUrl, group.AvatarUrl);
                Assert.Equal(updatedGroup.CreatedBy, group.CreatedBy);
                Assert.Equal(updatedGroup.CreatedDate, group.CreatedDate);
                Assert.Equal(updatedGroup.CustomProperties, group.CustomProperties);
                Assert.Equal(updatedGroup.Description, group.Description);
                Assert.Equal(updatedGroup.LastModifyBy, group.LastModifyBy);
                Assert.Equal(updatedGroup.LastModifyDate, group.LastModifyDate);
            }
            await _repository.RemoveRangeAsync(groupList);
        }

        [Fact]
        public async Task Should_Query_With_ListAsync()
        {
            var id = _group.Id;
            var group = await _repository
                     .Query<Group>()
                     .Where(e => e.Id == _group.Id)
                     .Select(e => new
                     {
                         e.Id,
                         e.CreatedBy,
                         e.CreatedDate
                     })
                     .ToListAsync()
                     .ConfigureAwait(false);
            group.ShouldNotBeEmpty();
            group.FirstOrDefault().Id.ShouldBe(_group.Id);
        }

        public void Dispose()
        {
            _repository.RemoveAsync(_group).Wait();
        }
    }
}
