// using Shouldly;
// using SugarChat.Core.Domain;
// using Xunit;
// using System.Threading.Tasks;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using SugarChat.Shared.Paging;
//
// namespace SugarChat.Database.MongoDb.IntegrationTest
// {
//     public class MongoDbRepositoryTest : TestBase
//     {
//         readonly Group _group;
//
//         public MongoDbRepositoryTest()
//         {
//             _group = new Group()
//             {
//                 AvatarUrl = "https://Avatar.jpg",
//                 CreatedBy = Guid.NewGuid().ToString(),
//                 CreatedDate = DateTime.UtcNow,
//                 CustomProperties = new Dictionary<string, string>(),
//                 Description = "A Test Group!",
//                 Id = Guid.NewGuid().ToString(),
//                 LastModifyDate = DateTime.UtcNow,
//                 LastModifyBy = Guid.NewGuid().ToString(),
//                 Name = "TestGroup"
//             };
//             Repository.AddAsync(_group).Wait();
//         }
//
//         [Fact]
//         public async Task Should_Insert_One_Group_And_Remove_It()
//         {
//             var newGroup = new Group()
//             {
//                 AvatarUrl = "https://Avatar.jpg",
//                 CreatedBy = Guid.NewGuid().ToString(),
//                 CreatedDate = DateTime.Now,
//                 CustomProperties = new Dictionary<string, string>(),
//                 Description = "A Test Group!",
//                 Id = Guid.NewGuid().ToString(),
//                 LastModifyDate = DateTime.Now,
//                 LastModifyBy = Guid.NewGuid().ToString(),
//                 Name = "NewTestGroup"
//             };
//             await Repository.AddAsync(newGroup);
//             var firstGroup = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
//             firstGroup.ShouldNotBeNull();
//             await Repository.RemoveAsync(newGroup);
//             var deleted = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == newGroup.Id);
//             deleted.ShouldBeNull();
//         }
//
//         [Fact]
//         public async Task Should_Insert_Many_Groups_And_Remove_Them()
//         {
//             List<Group> groups = new List<Group>
//             {
//                 new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "A Test Group!",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = "NewTestGroup1"
//                 },
//                 new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "A Test Group!",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = "NewTestGroup2"
//                 }
//             };
//             await Repository.AddRangeAsync(groups);
//             var ids = groups.Select(e => e.Id).ToList();
//             var groupList = await Repository.ToListAsync<Group>(e => ids.Contains(e.Id));
//             groupList.Count.ShouldBe(groups.Count);
//             await Repository.RemoveRangeAsync(groupList);
//             var deleted = await Repository.ToListAsync<Group>(e => ids.Contains(e.Id));
//             deleted.ShouldBeEmpty();
//         }
//
//         [Fact]
//         public async Task Should_FirstOrDefault_Group_Be_Not_Null()
//         {
//             var group = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
//             group.ShouldNotBeNull();
//         }
//
//         [Fact]
//         public async Task Should_FirstOrDefault_Group_Be_Null()
//         {
//             var group = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
//             group.ShouldBeNull();
//         }
//
//         [Fact]
//         public async Task Should_List_Group_Be_Empty()
//         {
//             var list = await Repository.ToListAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
//             list.ShouldBeEmpty();
//         }
//
//         [Fact]
//         public async Task Should_List_Group_Not_Be_Empty()
//         {
//             var list = await Repository.ToListAsync<Group>(e => e.Id == _group.Id);
//             list.ShouldNotBeEmpty();
//         }
//
//         [Fact]
//         public async Task Should_Any_Group_Be_False()
//         {
//             var any = await Repository.AnyAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
//             any.ShouldBeFalse();
//         }
//
//         [Fact]
//         public async Task Should_Any_Group_Be_True()
//         {
//             var any = await Repository.AnyAsync<Group>(e => e.Id == _group.Id);
//             any.ShouldBeTrue();
//         }
//
//         [Fact]
//         public async Task Should_Count_Group_Great_Than_0()
//         {
//             var any = await Repository.CountAsync<Group>(e => e.Id == _group.Id);
//             any.ShouldBeGreaterThan(0);
//         }
//
//         [Fact]
//         public async Task Should_Count_Group_Equivalent_0()
//         {
//             var any = await Repository.CountAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
//             any.ShouldBeEquivalentTo(0);
//         }
//
//         [Fact]
//         public Task Should_Single_Group_Throw_Exception()
//         {
//             return Should.ThrowAsync<Exception>(async () =>
//                 await Repository.SingleAsync<Group>(e => e.Id == Guid.NewGuid().ToString()));
//         }
//
//         [Fact]
//         public async Task Should_Single_Group_Not_Throw_Exception()
//         {
//             var single = await Repository.SingleAsync<Group>(e => e.Id == _group.Id);
//             single.ShouldNotBeNull();
//         }
//
//         [Fact]
//         public async Task Should_SingleOrDefault_Group_Be_Default()
//         {
//             var single = await Repository.SingleOrDefaultAsync<Group>(e => e.Id == Guid.NewGuid().ToString());
//             single.ShouldBe(default);
//         }
//
//         [Fact]
//         public async Task Should_SingleOrDefault_Group_Not_Be_Default()
//         {
//             var single = await Repository.SingleOrDefaultAsync<Group>(e => e.Id == _group.Id);
//             single.ShouldNotBeNull();
//         }
//
//         [Fact]
//         public async Task Should_Update_Group_Name()
//         {
//             var firstGroup = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
//             firstGroup.ShouldNotBeNull();
//             string updatedName = "UpdatedGroupName";
//             firstGroup.Name = updatedName;
//             await Repository.UpdateAsync(firstGroup);
//             var updatedGroup = await Repository.FirstOrDefaultAsync<Group>(e => e.Id == _group.Id);
//             Assert.Equal(updatedGroup.Name, updatedName);
//             Assert.Equal(updatedGroup.AvatarUrl, firstGroup.AvatarUrl);
//             Assert.Equal(updatedGroup.CreatedBy, firstGroup.CreatedBy);
//             Assert.Equal(updatedGroup.CreatedDate, firstGroup.CreatedDate);
//             Assert.Equal(updatedGroup.CustomProperties, firstGroup.CustomProperties);
//             Assert.Equal(updatedGroup.Description, firstGroup.Description);
//             Assert.Equal(updatedGroup.LastModifyBy, firstGroup.LastModifyBy);
//             Assert.Equal(updatedGroup.LastModifyDate, firstGroup.LastModifyDate);
//         }
//
//         [Fact]
//         public async Task Should_Update_Group_Name_By_Range()
//         {
//             List<Group> groups = new List<Group>
//             {
//                 new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "A Test Group!",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = "NewTestGroup1"
//                 },
//                 new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "A Test Group!",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = "NewTestGroup2"
//                 }
//             };
//             await Repository.AddRangeAsync(groups);
//             groups[0].Name = "UpdatedTestGroup1";
//             groups[1].Name = "UpdatedTestGroup2";
//             await Repository.UpdateRangeAsync(groups);
//             var ids = groups.Select(e => e.Id).ToList();
//             var groupList = await Repository.ToListAsync<Group>(e => ids.Contains(e.Id));
//             groupList.Count.ShouldBe(groups.Count);
//             foreach (var updatedGroup in groupList)
//             {
//                 var group = groups.FirstOrDefault(e => e.Id == updatedGroup.Id);
//                 Assert.Equal(updatedGroup.Name, group.Name);
//                 Assert.Equal(updatedGroup.AvatarUrl, group.AvatarUrl);
//                 Assert.Equal(updatedGroup.CreatedBy, group.CreatedBy);
//                 Assert.Equal(updatedGroup.CreatedDate, group.CreatedDate);
//                 Assert.Equal(updatedGroup.CustomProperties, group.CustomProperties);
//                 Assert.Equal(updatedGroup.Description, group.Description);
//                 Assert.Equal(updatedGroup.LastModifyBy, group.LastModifyBy);
//                 Assert.Equal(updatedGroup.LastModifyDate, group.LastModifyDate);
//             }
//
//             await Repository.RemoveRangeAsync(groupList);
//         }
//
//         [Fact]
//         public async Task Should_Get_Paged_Result()
//         {
//             await Repository.RemoveRangeAsync(await Repository.ToListAsync<Group>(o => true));
//             List<Group> groups = new List<Group>();
//             for (int i = 0; i < 30; i++)
//             {
//                 groups.Add(new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "Test Paging",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = i.ToString()
//                 });
//             }
//
//             await Repository.AddRangeAsync(groups);
//             PageSettings pageSettings = new() {PageNum = 2, PageSize = 10};
//             var result = await Repository.ToPagedListAsync<Group>(pageSettings, o => o.Description == "Test Paging");
//             result.Total.ShouldBe(30);
//             result.Result.Count().ShouldBe(10);
//             string.Join('-', result.Result.Select(o => int.Parse(o.Name)).OrderBy(o => o).Select(o => o.ToString()))
//                 .ShouldBe("10-11-12-13-14-15-16-17-18-19");
//         }
//
//         [Fact]
//         public async Task Should_Throw_Exception_When_PageSettings_Are_Null()
//         {
//             await Repository.RemoveRangeAsync(await Repository.ToListAsync<Group>(o => true));
//             List<Group> groups = new List<Group>();
//             for (int i = 0; i < 30; i++)
//             {
//                 groups.Add(new Group
//                 {
//                     AvatarUrl = "https://Avatar.jpg",
//                     CreatedBy = Guid.NewGuid().ToString(),
//                     CreatedDate = DateTime.Now,
//                     CustomProperties = new Dictionary<string, string>(),
//                     Description = "Test Paging",
//                     Id = Guid.NewGuid().ToString(),
//                     LastModifyDate = DateTime.Now,
//                     LastModifyBy = Guid.NewGuid().ToString(),
//                     Name = i.ToString()
//                 });
//             }
//
//             await Repository.AddRangeAsync(groups);
//             await Should.ThrowAsync<ArgumentException>(async () =>
//                 await Repository.ToPagedListAsync<Group>(null, o => o.Description == "Test Paging"));
//         }
//     }
// }