using Autofac;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.IntegrationTest.Services
{
    public class TestFixtureBase : TestBase
    {
        protected List<User> users = new List<User>();
        protected List<Group> groups = new List<Group>();
        protected List<Group2> group2s = new List<Group2>();
        protected List<GroupUser> groupUsers = new List<GroupUser>();
        private List<Friend> friends = new List<Friend>();

        public string userId = Guid.NewGuid().ToString();
        public string conversationId = Guid.NewGuid().ToString();


        public string groupId1 = Guid.NewGuid().ToString();
        public string groupId2 = Guid.NewGuid().ToString();
        public string groupId4 = Guid.NewGuid().ToString();
        public string groupId5 = Guid.NewGuid().ToString();
        public string groupId6 = Guid.NewGuid().ToString();

        public string userId1 = Guid.NewGuid().ToString();
        public string userId2 = Guid.NewGuid().ToString();
        public string userId3 = Guid.NewGuid().ToString();
        public string userId4 = Guid.NewGuid().ToString();
        public string userId5 = Guid.NewGuid().ToString();
        public string userId6 = Guid.NewGuid().ToString();
        public string userId7 = Guid.NewGuid().ToString();
        public string userId8 = Guid.NewGuid().ToString();
        public string userId9 = Guid.NewGuid().ToString();

        public TestFixtureBase()
        {
            GenerateTestCollections(Container.Resolve<IRepository>(), userId, conversationId);
        }
        public void GenerateTestCollections(IRepository repository, string userId, string conversationId)
        {
            var groupDic = new Dictionary<string, string>
            {
                { groupId1, "TestGroup1" },
                { groupId2, "TestGroup2" },
                { conversationId, "TestGroup3" },
                { groupId4, "TestGroup4" },
                { groupId5, "TestGroup5" },
                { groupId6, "TestGroup6" }
            };
            GenerateGroupCollection(repository, groupDic);
            repository.AddRangeAsync(groups, default(CancellationToken)).Wait();
            repository.AddRangeAsync(group2s, default(CancellationToken)).Wait();

            var userDic = new Dictionary<string, string>
            {
                { userId1, "TestUser1" },
                { userId2, "TestUser2" },
                { userId3, "TestUser3" },
                { userId4, "TestUser4" },
                { userId5, "TestUser5" },
                { userId6, "TestUser6" },
                { userId7, "TestUser7" },
                { userId8, "TestUser8" },
                { userId9, "TestUser9" },
                { userId,  "TestUser10" }
            };
            GenerateUserCollection(userDic);
            repository.AddRangeAsync(users, default(CancellationToken)).Wait();

            GenerateFriend(userId, userId9);
            repository.AddRangeAsync(friends, default(CancellationToken)).Wait();

            groupUsers.Add(GenerateGroupUser(repository, userId, groupId1));
            groupUsers.Add(GenerateGroupUser(repository, userId9, groupId1));
            groupUsers.Add(GenerateGroupUser(repository, userId8, groupId1));
            groupUsers.Add(GenerateGroupUser(repository, userId, groupId2));
            groupUsers.Add(GenerateGroupUser(repository, userId5, groupId2));
            groupUsers.Add(GenerateGroupUser(repository, userId6, groupId2));
            groupUsers.Add(GenerateGroupUser(repository, userId7, groupId2));
            groupUsers.Add(GenerateGroupUser(repository, userId, conversationId, 2));
            groupUsers.Add(GenerateGroupUser(repository, userId9, conversationId, 1));
            groupUsers.Add(GenerateGroupUser(repository, userId1, groupId4, 3));
            groupUsers.Add(GenerateGroupUser(repository, userId, groupId4, 3));
            groupUsers.Add(GenerateGroupUser(repository, userId2, groupId4, 3, new Dictionary<string, string> { { "userType", "admin" } }));
            groupUsers.Add(GenerateGroupUser(repository, userId4, groupId4, 4));
            groupUsers.Add(GenerateGroupUser(repository, userId3, groupId4, 3, new Dictionary<string, string> { { "userType", "admin" } }));
            groupUsers.Add(GenerateGroupUser(repository, userId, groupId5, 1));
            groupUsers.Add(GenerateGroupUser(repository, userId1, groupId5, 1));
            repository.AddRangeAsync(groupUsers, default(CancellationToken)).Wait();

            GenerateMessage(repository, conversationId, "我通过了你的朋友验证请求,现在我们可以开始聊天了", 0, userId9, new Dictionary<string, string> { { "text", "test1" }, { "order", "11" } }, "{\"text\":\"test1\",\"order\":\"11\"}");
            GenerateMessage(repository, conversationId, "你好", 0, userId9, new Dictionary<string, string> { { "text", "test2" }, { "order", "12" } }, "{\"text\":\"test2\",\"order\":\"12\"}");
            GenerateMessage(repository, conversationId, "[图片]", 1, userId, new Dictionary<string, string> { { "text", "test3" }, { "order", "13" } }, "{\"text\":\"test3\",\"order\":\"13\"}");
            GenerateMessage(repository, groupId4, "TestUser1邀请TestUser2加入了群聊", 0, userId, new Dictionary<string, string> { { "text", "test4" }, { "order", "24" } }, "{\"text\":\"test4\",\"order\":\"24\"}");
            GenerateMessage(repository, groupId4, "Congratulations! Your friend 六角恐龙～+. had completed an order, you are awarded 100 points from QC Test Store!", 0, userId1, new Dictionary<string, string> { { "text", "test5" }, { "order", "25" } }, "{\"text\":\"test5\",\"order\":\"25\"}");
            GenerateMessage(repository, groupId4, "是啊,又到了不动都能出汗的季节了", 0, userId2, new Dictionary<string, string> { { "text", "test6" }, { "order", "26" } }, "{\"text\":\"test6\",\"order\":\"26\"}");
            GenerateMessage(repository, groupId4, "谁说不是呢", 0, userId3, new Dictionary<string, string> { { "text", "test7" }, { "order", "37" } }, "{\"text\":\"test7\",\"order\":\"37\"}");
            GenerateMessage(repository, groupId5, "999", 0, userId1, new Dictionary<string, string> { { "text", "test9" }, { "order", "9" } }, "{\"text\":\"test9\",\"order\":\"9\"}");
            GenerateMessage(repository, groupId5, "888", 0, userId, new Dictionary<string, string> { { "text", "test8" }, { "order", "8" } }, "{\"text\":\"test8\",\"order\":\"8\"}");
        }

        private void GenerateGroupCollection(IRepository repository, Dictionary<string, string> groupDic)
        {
            for (int i = 0; i < groupDic.Count; i++)
            {
                groups.Add(new Group
                {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    Description = "A Test Group!",
                    Id = groupDic.ElementAt(i).Key,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = groupDic.ElementAt(i).Value,
                    Type = 10,
                    CustomProperties = new Dictionary<string, string> { { "A", i.ToString() + "AB" } }
                });
                group2s.Add(new Group2
                {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    Description = "A Test Group!",
                    Id = groupDic.ElementAt(i).Key,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = groupDic.ElementAt(i).Value,
                    Type = 10,
                    CustomProperties = new Dictionary<string, string> { { "A", i.ToString() + "AB" } }
                });
                repository.AddAsync(new GroupCustomProperty
                {
                    GroupId = groupDic.ElementAt(i).Key,
                    Key = "A",
                    Value = i.ToString() + "AB"
                }, default).Wait();
                repository.AddAsync(new GroupCustomProperty
                {
                    GroupId = groupDic.ElementAt(i).Key,
                    Key = "B",
                    Value = (i % 2).ToString() + "BC"
                }, default).Wait();

            }
        }
        private void GenerateUserCollection(Dictionary<string, string> userDic)
        {
            foreach (var m in userDic)
            {
                users.Add(new User
                {
                    Id = m.Key,
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    LastModifyDate = DateTimeOffset.Now,
                    DisplayName = m.Value,
                    AvatarUrl = "",
                });
            }
        }
        private Friend GenerateFriend(string userId, string friendId)
        {
            return new Friend
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                LastModifyDate = DateTimeOffset.Now,
                UserId = userId, //用户10
                FriendId = friendId, //用户9
                BecomeFriendAt = DateTimeOffset.Now
            };
        }
        private GroupUser GenerateGroupUser(IRepository repository, string userId, string groupId, int unreadCount = 0, Dictionary<string, string> customProperties = null)
        {
            var groupUser = new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                LastModifyDate = DateTimeOffset.Now,
                UserId = userId, //用户10
                GroupId = groupId,//组1
                UnreadCount = unreadCount
            };

            var group = repository.Query<Group>().Where(x => x.Id == groupId).SingleOrDefault();
            var groupCustomProperties = repository.Query<GroupCustomProperty>().Where(x => x.GroupId == groupId).ToList();
            if (group != null)
                groupUser.GroupType = group.Type;
            if (customProperties == null)
                customProperties = new Dictionary<string, string>();
            foreach (var groupCustomProperty in groupCustomProperties)
            {
                customProperties.Add(groupCustomProperty.Key, groupCustomProperty.Value);
            }
            groupUser.CustomProperties = customProperties;

            var mapper = Container.Resolve<IMapper>();
            var group2 = repository.Query<Group2>().Where(x => x.Id == groupId).SingleOrDefault();
            group2.GroupUsers.Add(mapper.Map<GroupUser2>(groupUser));
            repository.UpdateAsync(group2);

            return groupUser;
        }
        private void GenerateMessage(IRepository repository, string groupId, string content, int type, string sentBy, Dictionary<string, string> customProperties, string payload)
        {
            var messageId = Guid.NewGuid().ToString();
            var message = new Core.Domain.Message
            {
                Id = messageId,
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                LastModifyDate = DateTimeOffset.Now,
                GroupId = groupId,
                Content = content,
                Type = type,
                SentBy = sentBy, //用户3
                SentTime = DateTimeOffset.Now,
                IsSystem = true,
                Payload = payload,
                CustomProperties = customProperties
            };
            repository.AddAsync(message, default(CancellationToken)).Wait();

            var mapper = Container.Resolve<IMapper>();
            var group2 = repository.Query<Group2>().Where(x => x.Id == groupId).SingleOrDefault();
            group2.Messages.Add(mapper.Map<Message2>(message));
            repository.UpdateAsync(group2);
        }
    }
}
