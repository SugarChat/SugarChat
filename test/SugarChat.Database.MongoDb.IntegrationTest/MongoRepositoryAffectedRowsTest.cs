using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.Database.MongoDb.IntegrationTest
{
    public class MongoRepositoryAffectedRowsTest : TestBase
    {
        public MongoRepositoryAffectedRowsTest(DatabaseFixture dbFixture) : base(dbFixture)
        {
        }

        [Fact]
        public async Task Should_Add_3_Groups()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            try
            {

                var total = await Repository.AddRangeAsync(groups);
                total.ShouldBe(groups.Count);
            }
            finally
            {
                await Repository.RemoveRangeAsync(groups);
            }
        }

        [Fact]
        public async Task Should_Update_1_Group()
        {
            var group = new Group
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
            try
            {
                var addedTotal = await Repository.AddAsync(group);
                addedTotal.ShouldBe(1);
                var updatedTotal = await Repository.UpdateAsync(group);
                updatedTotal.ShouldBe(1);

            }
            finally
            {
                var deletedTotoal = await Repository.RemoveAsync(group);
                deletedTotoal.ShouldBe(1);
            }
        }

        [Fact]
        public async Task Should_Update_2_Groups()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            try
            {
                var addedTotal = await Repository.AddRangeAsync(groups);
                groups[0].Id = Guid.NewGuid().ToString();
                foreach (var group in groups)
                {
                    group.Name = "New" + group.Name;
                }
                var updatedTotal = await Repository.UpdateRangeAsync(groups);
                updatedTotal.ShouldBe(2);
            }
            finally
            {
                await Repository.RemoveRangeAsync(groups);
            }
        }

        [Fact]
        public async Task Should_Update_None_Group()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            try
            {
                var addedTotal = await Repository.AddRangeAsync(groups);
                foreach (var group in groups)
                {
                    group.Name = "New" + group.Name;
                    group.Id = Guid.NewGuid().ToString();
                }
                var updatedTotal = await Repository.UpdateRangeAsync(groups);
                updatedTotal.ShouldBe(0);
            }
            finally
            {
                await Repository.RemoveRangeAsync(groups);
            }
        }

        [Fact]
        public async Task Should_Update_3_Groups()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            try
            {
                var addedTotal = await Repository.AddRangeAsync(groups);
                foreach (var group in groups)
                {
                    group.Name = "New" + group.Name;
                }
                var updatedTotal = await Repository.UpdateRangeAsync(groups);
                updatedTotal.ShouldBe(groups.Count);
            }
            finally
            {
                await Repository.RemoveRangeAsync(groups);
            }
        }

        [Fact]
        public async Task Should_Remove_3_Groups()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            await Repository.AddRangeAsync(groups);
            var deletedTotal = await Repository.RemoveRangeAsync(groups);
            deletedTotal.ShouldBe(3);
        }

        [Fact]
        public async Task Should_Remove_None_Groups_After_Remove()
        {
            List<Group> groups = new List<Group>
            {
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup1"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup2"
                    },
                    new Group
                    {
                        AvatarUrl = "https://Avatar.jpg",
                        CreatedBy = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        CustomProperties = new Dictionary<string, string>(),
                        Description = "A Test Group!",
                        Id = Guid.NewGuid().ToString(),
                        LastModifyDate = DateTime.Now,
                        LastModifyBy = Guid.NewGuid().ToString(),
                        Name = "TestGroup3"
                    },
                };
            try
            {
                await Repository.AddRangeAsync(groups);
                var deletedTotal = await Repository.RemoveRangeAsync(groups);
                deletedTotal.ShouldBe(3);
            }
            finally
            {
                var deletedTotal = await Repository.RemoveRangeAsync(groups);
                deletedTotal.ShouldBe(0);
            }
        }
    }
}
