using Autofac;
using Mediator.Net;
using Shouldly;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SugarChat.IntegrationTest.Services.Conversations
{
    public class ConversationServiceFixture : TestBase
    {

        private List<User> users = new List<User>();
        private List<Group> groups = new List<Group>();
        private List<GroupUser> groupUsers = new List<GroupUser>();
        private List<Friend> friends = new List<Friend>();
        private List<Core.Domain.Message> messages = new List<Core.Domain.Message>();
        private string userId = Guid.NewGuid().ToString();
        private string conversationId = Guid.NewGuid().ToString();

        public ConversationServiceFixture()
        {
            GenerateTestCollections(Container.Resolve<IRepository>(), userId, conversationId);
        }

        [Fact]
        public async Task ShouldGetUserConversations()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {              
                var groupIds = groups.Select(x => x.Id).ToList();
                var groupList = await repository.ToListAsync<Group>(x => groupIds.Contains(x.Id));
                groupList.Count.ShouldBe(groups.Count);

                var userIds = users.Select(x => x.Id).ToList();
                var userList = await repository.ToListAsync<User>(x => userIds.Contains(x.Id));
                userList.Count.ShouldBe(users.Count);

                var friendIds = friends.Select(x => x.Id).ToList();
                var friendList = await repository.ToListAsync<Friend>(x => friendIds.Contains(x.Id));
                friendList.Count.ShouldBe(friends.Count);

                var groupUserIds = groupUsers.Select(x => x.Id).ToList();
                var groupUserList = await repository.ToListAsync<GroupUser>(x => groupUserIds.Contains(x.Id));
                groupUserList.Count.ShouldBe(groupUsers.Count);

                var messageIds = messages.Select(x => x.Id).ToList();
                var messageList = await repository.ToListAsync<Core.Domain.Message>(x => messageIds.Contains(x.Id));
                messageList.Count.ShouldBe(messages.Count);

                var reponse = await mediator.RequestAsync<GetConversationListRequest, GetConversationListResponse>
                (new GetConversationListRequest { UserId = userId });

                reponse.Result.Count().ShouldBe(2);
            });
        }

        [Fact]
        public async Task ShouldSetConversationMessagesRead()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                await mediator.SendAsync(new SetMessageAsReadCommand
                {
                    ConversationId = conversationId,
                    UserId = userId

                }, default(CancellationToken));

                var request = new GetConversationListRequest()
                {
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationListRequest, GetConversationListResponse>(request);
                response.Result.Where(x => x.ConversationID == conversationId)
                .FirstOrDefault().UnreadCount.ShouldBe(0);
            });
        }

        [Fact]
        public async Task ShouldGetConversationProfile()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {
                var request = new GetConversationProfileRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId
                };
                var response = await mediator.RequestAsync<GetConversationProfileRequest, GetConversationProfileResponse>(request);
                response.Result.Name.ShouldBe("TestGroup3");
            });
        }

        [Fact]
        public async Task ShouldGetMessageList()
        {
            await Run<IMediator, IRepository>(async (mediator, repository) =>
            {                
                var request = new GetMessageListRequest()
                {
                    ConversationId = conversationId,
                    UserId = userId,
                    NextReqMessageId = "",
                    Count = 5
                };
                var response = await mediator.RequestAsync<GetMessageListRequest, GetMessageListResponse>(request);
                response.Result.Count().ShouldBe(3);
                response.Result.First().Content.ShouldBe("[图片]");
            });
        }

        private void GenerateTestCollections(IRepository repository, string userId, string conversationId)
        {
            var groupId1 = Guid.NewGuid().ToString();
            var groupId2 = Guid.NewGuid().ToString();
            var groupId4 = Guid.NewGuid().ToString();
            var groupId5 = Guid.NewGuid().ToString();
            var groupId6 = Guid.NewGuid().ToString();

            groups = new List<Group>
            {
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = groupId1,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup1"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id =groupId2,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup2"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = conversationId,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup3"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = groupId4,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup4"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = groupId5,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup5"
               },
               new Group
               {
                    AvatarUrl = "",
                    CreatedBy = Guid.NewGuid().ToString(),
                    CreatedDate = DateTimeOffset.Now,
                    CustomProperties = new Dictionary<string, string>(),
                    Description = "A Test Group!",
                    Id = groupId6,
                    LastModifyDate = DateTimeOffset.Now,
                    LastModifyBy = Guid.NewGuid().ToString(),
                    Name = "TestGroup6"
               }
            };

            var userId1 = Guid.NewGuid().ToString();
            var userId2 = Guid.NewGuid().ToString();
            var userId3 = Guid.NewGuid().ToString();
            var userId4 = Guid.NewGuid().ToString();
            var userId5 = Guid.NewGuid().ToString();
            var userId6 = Guid.NewGuid().ToString();
            var userId7 = Guid.NewGuid().ToString();
            var userId8 = Guid.NewGuid().ToString();
            var userId9 = Guid.NewGuid().ToString();
            users = new List<User>
            {
                new User
                {
                   Id=userId1,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser1",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId2,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser2",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId3,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser3",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId4,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser4",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId5,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser5",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId6,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser6",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId7,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser7",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId8,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser8",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId9,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser9",
                   AvatarUrl="",
                },
                new User
                {
                   Id=userId,
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   DisplayName="TestUser10",
                   AvatarUrl="",
                }
            };

            friends = new List<Friend>
            {
                new Friend
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId, //用户10
                   FriendId=userId9, //用户9
                   BecomeFriendAt=DateTimeOffset.Now
                }

            };

            groupUsers = new List<GroupUser>
            {
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId, //用户10
                   GroupId=groupId1,//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId9, //用户9
                   GroupId=groupId1,//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId8, //用户8
                   GroupId=groupId1,//组1
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId, //用户10
                   GroupId=groupId2,//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId5, //用户5
                   GroupId=groupId2,//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId6, //用户6
                   GroupId=groupId2,//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId7, //用户7
                   GroupId=groupId2,//组2
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId, //用户10
                   GroupId=conversationId,//组3
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId9, //用户9
                   GroupId=conversationId,//组3
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId1, //用户1
                   GroupId=groupId4,//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId, //用户10
                   GroupId=groupId4,//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId2, //用户2
                   GroupId=groupId4,//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId4, //用户4                                                                                                                                                                                                       
                   GroupId=groupId4,//组4
                },
                new GroupUser
                {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   UserId=userId3, //用户3
                   GroupId=groupId4,//组4
                },
            };

            messages = new List<Core.Domain.Message>
            {
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=conversationId,
                   Content="我通过了你的朋友验证请求,现在我们可以开始聊天了",
                   ParsedContent="我通过了你的朋友验证请求,现在我们可以开始聊天了",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy=userId9,    //用户9通过用户10的好友发送的一条消息
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=conversationId,
                   Content="你好",
                   ParsedContent="你好",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy=userId9,    //用户9
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=conversationId,
                   Content="[图片]",
                   ParsedContent="图片转化后的内容",
                   Type=MessageType.Image,
                   SubType=0,
                   SentBy=userId,    //用户10
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=false
               },

               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=groupId4,
                   Content="TestUser1邀请TestUser2加入了群聊",
                   ParsedContent="TestUser1邀请TestUser2加入了群聊",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy="",
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=groupId4,
                   Content="今天真热啊",
                   ParsedContent="今天真热啊",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy=userId1, //用户1
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=groupId4,
                   Content="是啊,又到了不动都能出汗的季节了",
                   ParsedContent="是啊,又到了不动都能出汗的季节了",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy=userId2, //用户2
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               },
               new Core.Domain.Message
               {
                   Id=Guid.NewGuid().ToString(),
                   CreatedBy=Guid.NewGuid().ToString(),
                   CreatedDate=DateTimeOffset.Now,
                   LastModifyBy=Guid.NewGuid().ToString(),
                   CustomProperties=new Dictionary<string, string>(),
                   LastModifyDate=DateTimeOffset.Now,
                   GroupId=groupId4,
                   Content="谁说不是呢",
                   ParsedContent="谁说不是呢",
                   Type=MessageType.Text,
                   SubType=0,
                   SentBy=userId3, //用户3
                   SentTime=DateTimeOffset.Now,
                   IsDel=false,
                   IsSystem=true
               }
            };

            repository.AddRangeAsync(users, default(CancellationToken)).Wait();
            repository.AddRangeAsync(groups, default(CancellationToken)).Wait();
            repository.AddRangeAsync(friends, default(CancellationToken)).Wait();
            repository.AddRangeAsync(groupUsers, default(CancellationToken)).Wait();
            repository.AddRangeAsync(messages, default(CancellationToken)).Wait();
        }
        
    }
}
