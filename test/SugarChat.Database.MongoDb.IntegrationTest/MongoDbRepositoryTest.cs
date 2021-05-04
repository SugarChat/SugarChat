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

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class MongoDbRepositoryTest : TestBase
    {
        readonly IRepository repository;
        public MongoDbRepositoryTest()
        {
            MongoDbSettings settings = new MongoDbSettings();
            _configuration.GetSection("MongoDb")
                          .Bind(settings);
            repository = new MongoDbRepository(settings);
        }

        [Fact]
        public void Should_Repository_Not_Null()
        {
            repository.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Insert_One_Group()
        {
            var id = Guid.NewGuid().ToString();
            Group group = new Group()
            {
                AvatarUrl = "https://Avatar.jpg",
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CustomProperties = new Dictionary<string, string>(),
                Description = "A Test Group!",
                Id = id,
                LastModifyDate = DateTime.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                Name = "TestGroup"
            };
            await repository.AddAsync(group, default(CancellationToken));
            var firstGroup = await repository.FirstOrDefaultAsync<Group>(e => e.Id == id);
            firstGroup.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_FirstOrDefault_Group_Be_Not_Null()
        {
            var group = await repository.FirstOrDefaultAsync<Group>();
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
            var list = await repository.ToListAsync<Group>();
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
            var any = await repository.AnyAsync<Group>();
            any.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Count_Group_Great_Than_0()
        {
            var any = await repository.CountAsync<Group>();
            any.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Count_Group_Equivalent_0()
        {
            var any = await repository.CountAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
            any.ShouldBeEquivalentTo(0);
        }
    }
}
