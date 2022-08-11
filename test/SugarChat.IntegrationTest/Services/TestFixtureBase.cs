using Autofac;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SugarChat.IntegrationTest.Services
{
    public class TestFixtureBase : TestBase
    {
        protected List<User> users = new List<User>();
        protected List<Group> groups = new List<Group>();
        protected List<GroupUser> groupUsers = new List<GroupUser>();
        private List<Friend> friends = new List<Friend>();
        private List<Core.Domain.Message> messages = new List<Core.Domain.Message>();

        public string userId = Guid.NewGuid().ToString();
        public string conversationId = Guid.NewGuid().ToString();


        public string groupId1 = Guid.NewGuid().ToString();
        public string groupId2 = Guid.NewGuid().ToString();
        public string groupId4 = Guid.NewGuid().ToString();
        public string groupId5 = Guid.NewGuid().ToString();
        public string groupId6 = Guid.NewGuid().ToString();

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
            GenerateGroupCollection(groupDic);
            repository.AddRangeAsync(groups, default(CancellationToken)).Wait();

            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var userId3 = Guid.NewGuid().ToString();
            var userId4 = Guid.NewGuid().ToString();
            var userId5 = Guid.NewGuid().ToString();
            var userId6 = Guid.NewGuid().ToString();
            var userId7 = Guid.NewGuid().ToString();
            var userId8 = Guid.NewGuid().ToString();
            var userId9 = Guid.NewGuid().ToString();

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

            groupUsers.Add(GenerateGroupUser(userId, groupId1));
            groupUsers.Add(GenerateGroupUser(userId9, groupId1));
            groupUsers.Add(GenerateGroupUser(userId8, groupId1));
            groupUsers.Add(GenerateGroupUser(userId, groupId2));
            groupUsers.Add(GenerateGroupUser(userId5, groupId2));
            groupUsers.Add(GenerateGroupUser(userId6, groupId2));
            groupUsers.Add(GenerateGroupUser(userId7, groupId2));
            groupUsers.Add(GenerateGroupUser(userId, conversationId));
            groupUsers.Add(GenerateGroupUser(userId9, conversationId));
            groupUsers.Add(GenerateGroupUser(userId1, groupId4));
            groupUsers.Add(GenerateGroupUser(userId, groupId4));
            groupUsers.Add(GenerateGroupUser(userId2, groupId4));
            groupUsers.Add(GenerateGroupUser(userId4, groupId4));
            groupUsers.Add(GenerateGroupUser(userId3, groupId4));
            groupUsers.Add(GenerateGroupUser(userId, groupId5));
            repository.AddRangeAsync(groupUsers, default(CancellationToken)).Wait();

            messages.Add(GenerateMessage(conversationId, "我通过了你的朋友验证请求,现在我们可以开始聊天了", 0, userId9, new Dictionary<string, string> { { "text", "test1" }, { "order", "11" } }, "{\"text\":\"test1\",\"order\":\"11\"}"));
            messages.Add(GenerateMessage(conversationId, "你好", 0, userId9, new Dictionary<string, string> { { "text", "test2" }, { "order", "12" } }, "{\"text\":\"test2\",\"order\":\"12\"}"));
            messages.Add(GenerateMessage(conversationId, "[图片]", 1, userId, new Dictionary<string, string> { { "text", "test3" }, { "order", "13" } }, "{\"text\":\"test3\",\"order\":\"13\"}"));
            messages.Add(GenerateMessage(groupId4, "TestUser1邀请TestUser2加入了群聊", 0, userId, new Dictionary<string, string> { { "text", "test4" }, { "order", "24" } }, "{\"text\":\"test4\",\"order\":\"24\"}"));
            messages.Add(GenerateMessage(groupId4, "Congratulations! Your friend 六角恐龙～+. had completed an order, you are awarded 100 points from QC Test Store!", 0, userId1, new Dictionary<string, string> { { "text", "test5" }, { "order", "25" } }, "{\"text\":\"test5\",\"order\":\"25\"}"));
            messages.Add(GenerateMessage(groupId4, "是啊,又到了不动都能出汗的季节了", 0, userId2, new Dictionary<string, string> { { "text", "test6" }, { "order", "26" } }, "{\"text\":\"test6\",\"order\":\"26\"}"));
            messages.Add(GenerateMessage(groupId4, "谁说不是呢", 0, userId3, new Dictionary<string, string> { { "text", "test7" }, { "order", "37" } }, "{\"text\":\"test7\",\"order\":\"37\"}"));
            messages.Add(GenerateMessage(groupId5, "888", 0, userId, new Dictionary<string, string> { { "text", "test8" }, { "order", "8" } }, "{\"text\":\"test8\",\"order\":\"8\"}"));
            repository.AddRangeAsync(messages, default(CancellationToken)).Wait();
        }

        private void GenerateGroupCollection(Dictionary<string, string> groupDic)
        {
            foreach (var m in groupDic)
            {
                groups.Add(new Group
                {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = m.Key,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = m.Value,
                    Type = 10
                });
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
                    CustomProperties = new Dictionary<string, string>(),
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
                CustomProperties = new Dictionary<string, string>(),
                LastModifyDate = DateTimeOffset.Now,
                UserId = userId, //用户10
                FriendId = friendId, //用户9
                BecomeFriendAt = DateTimeOffset.Now
            };
        }
        private GroupUser GenerateGroupUser(string userId, string groupId)
        {
            return new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                CustomProperties = new Dictionary<string, string>(),
                LastModifyDate = DateTimeOffset.Now,
                UserId = userId, //用户10
                GroupId = groupId,//组1
            };
        }
        private Core.Domain.Message GenerateMessage(string groupId, string content, int type, string sentBy, Dictionary<string, string> customProperties, string payload)
        {
            return new Core.Domain.Message
            {
                Id = Guid.NewGuid().ToString(),
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTimeOffset.Now,
                LastModifyBy = Guid.NewGuid().ToString(),
                CustomProperties = customProperties,
                LastModifyDate = DateTimeOffset.Now,
                GroupId = groupId,
                Content = content,
                Type = type,
                SentBy = sentBy, //用户3
                SentTime = DateTimeOffset.Now,
                IsSystem = true,
                Payload = payload
            };
        }



    }
}
